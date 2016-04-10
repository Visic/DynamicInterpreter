using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace REPL { 
    public class NavigableBuffer<T> {
        public class Mark : IComparable {
            public Mark(int bufferIndex, int curX, int curY) {
                BufferIndex = bufferIndex;
                CursorX = curX;
                CursorY = curY;
            }

            public int BufferIndex { get; }
            public int CursorX { get; }
            public int CursorY { get; }

            public int CompareTo(object obj) {
                if (obj is int) return BufferIndex.CompareTo((int)obj);
                return BufferIndex.CompareTo(((Mark)obj).BufferIndex);
            }

            public override string ToString() {
                return $"{BufferIndex}  ({CursorX}, {CursorY})";
            }
        }

        public event EventHandler<Tuple<int, int>> PositionChanged;
        public event EventHandler<Tuple<int, int>> DimensionsChanged;
        public event EventHandler LineCleared;
        public event EventHandler<T[]> ValueWritten;
        OrderedList<Mark> _marks = new OrderedList<Mark>();
        T[] _buffer;
        bool _raisePosChanged;

        public NavigableBuffer() {
            CursorX = new Property<int>((oldX, newX) => newX >= 0 && newX < Width.Value, x => { if(_raisePosChanged) RaisePosChanged(); });
            CursorY = new Property<int>((oldY, newY) => newY >= 0 && newY < Height.Value, y => { if(_raisePosChanged) RaisePosChanged(); });
            Width = new Property<int>(
                (oldW, newW) => {
                    if (newW < 0) return false;
                    if(newW < oldW && CursorX.Value > newW) CursorX.Value = newW;
                    Array.Resize(ref _buffer, newW * Height.Value);
                    return true;
                }, 
                w => DimensionsChanged?.Invoke(this, Tuple.Create(Width.Value, Height.Value))
            );
            Height = new Property<int>(
                (oldY, newY) => {
                    if (newY < 0) return false;
                    if(newY < oldY && CursorY.Value > newY) CursorY.Value = newY;
                    Array.Resize(ref _buffer, Width.Value * newY);
                    return true;
                }, 
                h => DimensionsChanged?.Invoke(this, Tuple.Create(Width.Value, Height.Value))
            );
            _buffer = new T[Width.Value * Height.Value];
        }

        public Property<int> CursorX { get; }
        public Property<int> CursorY { get; }
        public Property<int> Width { get; }
        public Property<int> Height { get; }

        public void Write(IEnumerable<T> vals, bool advanceCursor = true) {
            Write(advanceCursor, vals.ToArray());
        }

        public void Write(bool advanceCursor = true, params T[] vals) {
            if (vals.Length == 0) return;
            var curX = CursorX.Value;
            var curY = CursorY.Value;

            _raisePosChanged = false;
            foreach(var ele in vals) {
                _buffer[CurrentPos()] = ele;
                MoveRight();
            }
            ValueWritten?.Invoke(this, vals);
            if (!advanceCursor) {
                CursorX.Value = curX;
                CursorY.Value = curY;
            }
            RaisePosChanged();
            _raisePosChanged = true;
        }

        public void MarkPos() {
            _marks.Add(new Mark(CurrentPos(), CursorX.Value, CursorY.Value));
        }

        public Option<int> DistanceToNextMark() {
            return GetNextMarkIndex().Apply(x => x - CurrentPos());
        }

        public Option<int> DistanceToPreviousMark() {
            return GetPreviousMarkIndex().Apply(x => CurrentPos() - x);
        }

        public void MoveToNextMark() {
            MoveToMark(GetNextMarkIndex);
        }

        public void MoveToPreviousMark() {
            MoveToMark(GetPreviousMarkIndex);
        }

        public void VisitToNextMark(Action<Tuple<int, int>, Option<T>, Option<T>> visitor, bool actuallyMove = true) {
            VisitToMark(GetNextMarkIndex, visitor, actuallyMove);
        }

        public void VisitToPreviousMark(Action<Tuple<int, int>, Option<T>, Option<T>> visitor, bool actuallyMove = true) {
            VisitToMark(GetPreviousMarkIndex, visitor, actuallyMove);
        }

        public void RemoveAllMarks() {
            _marks.Clear();
        }

        public bool RemoveCurrentMark() {
            return RemoveMark(GetCurrentMarkIndex);
        }

        public bool RemoveNextMark() {
            return RemoveMark(GetNextMarkIndex);
        }

        public bool RemovePreviousMark() {
            return RemoveMark(GetPreviousMarkIndex);
        }

        public T[] GetToNextMark() {
            return GetToMark(GetNextMarkIndex);
        }

        public T[] GetToPreviousMark() {
            return GetToMark(GetPreviousMarkIndex);
        }

        public void ClearToNextMark() {
            ClearToMark(GetNextMarkIndex);
        }

        public void ClearToPreviousMark() {
            ClearToMark(GetPreviousMarkIndex);
        }

        public T[] GetLine() {
            var result = new T[Width.Value];
            Array.Copy(_buffer, CursorY.Value * Width.Value, result, 0, Width.Value);
            return result;
        }

        public void ClearLine() {
            Array.Clear(_buffer, CursorY.Value * Width.Value, Width.Value);
            CursorX.Value = 0;
            LineCleared?.Invoke(this, EventArgs.Empty);
        }

        public void Clear() {
            Array.Clear(_buffer, 0, _buffer.Length);
            CursorX.Value = 0;
            CursorY.Value = 0;
        }

        public bool IsOnMark() {
            return GetCurrentMarkIndex().IsSome;
        }

        public bool SetCursorPos(int x, int y) {
            var curX = CursorX.Value;
            var curY = CursorY.Value;
            CursorX.Value = x;
            CursorY.Value = y;

            if (CursorX.Value != x || CursorY.Value != y) {
                CursorX.Value = curX;
                CursorY.Value = curY;
                return false;
            }
            return true;
        }

        public bool MoveLeft(bool stopAtMark = false) {
            var maybeRestriction = GetPreviousBounds(stopAtMark, false);
            if (maybeRestriction.IsNone) return false;
            if (CursorX.Value <= maybeRestriction.Value.Item1 && CursorY.Value <= maybeRestriction.Value.Item2) return false;

            if (CursorX.Value == 0) {
                CursorY.Value -= 1;
                CursorX.Value = Width.Value - 1;
            } else {
                CursorX.Value -= 1;
            }
            return true;
        }

        public bool MoveRight(bool stopAtMark = false) {
            var maybeRestriction = GetNextBounds(stopAtMark, false);
            if (maybeRestriction.IsNone) return false;
            if (CursorX.Value >= maybeRestriction.Value.Item1 && CursorY.Value >= maybeRestriction.Value.Item2) return false;

            if (CursorX.Value == Width.Value - 1) {
                CursorY.Value += 1;
                CursorX.Value = 0;
            } else {
                CursorX.Value += 1;
            }
            return true;
        }

        public bool MoveUp(bool stopAtMark = false) {
            var maybeRestriction = GetPreviousBounds(stopAtMark, false);
            if (maybeRestriction.IsNone || CursorY.Value <= maybeRestriction.Value.Item2) return false;

            CursorY.Value -= 1;
            if (CursorX.Value < maybeRestriction.Value.Item1) CursorX.Value = maybeRestriction.Value.Item1;
            return true;
        }

        public bool MoveDown(bool stopAtMark = false) {
            var maybeRestriction = GetNextBounds(stopAtMark, false);
            if (maybeRestriction.IsNone || CursorY.Value >= maybeRestriction.Value.Item2) return false;

            CursorY.Value += 1;
            if (CursorX.Value > maybeRestriction.Value.Item1) CursorX.Value = maybeRestriction.Value.Item1;
            return true;
        }

        private Option<Tuple<int, int>> GetNextBounds(bool stopAtMark, bool ignoreNoMark) {
            Option<int> maybeIndex = new None();
            if (stopAtMark) maybeIndex = GetNextMarkIndex().Apply(index => _marks[index].BufferIndex);
            if (!stopAtMark || (maybeIndex.IsNone && ignoreNoMark)) maybeIndex = Width.Value * Height.Value - 1;
            return maybeIndex.Apply(index => GetPosFromIndex(index));
        }

        private Option<Tuple<int, int>> GetPreviousBounds(bool stopAtMark, bool ignoreNoMark) {
            Option<int> maybeIndex = new None();
            if (stopAtMark) maybeIndex = GetPreviousMarkIndex().Apply(index => _marks[index].BufferIndex);
            if (!stopAtMark || (maybeIndex.IsNone && ignoreNoMark)) maybeIndex = 0;
            return maybeIndex.Apply(index => GetPosFromIndex(index));
        }

        //Action<pos, previous, current>
        private void VisitToMark(Func<Option<int>> getMarkIndex, Action<Tuple<int, int>, Option<T>, Option<T>> visitor, bool actuallyMove) {
            getMarkIndex().Apply(index => {
                var startPos = CurrentPos();
                var endPos = _marks[index].BufferIndex;
                var direction = startPos > endPos ? -1 : 1;
                for(int i = startPos; i != endPos + direction; i += direction) {
                    var curPos = GetPosFromIndex(i);
                    if (actuallyMove) {
                        CursorX.Value = curPos.Item1;
                        CursorY.Value = curPos.Item2;
                    }

                    if (i == startPos) visitor(curPos, new None(), _buffer[i]);
                    else if (i == endPos) visitor(curPos, _buffer[i], new None());
                    else visitor(curPos, _buffer[i - direction], _buffer[i]);
                }
            });
        }

        private void ClearToMark(Func<Option<int>> getMarkIndex) {
            getMarkIndex().Apply(index => {
                var mark = _marks[index];
                var startPos = mark.BufferIndex < CurrentPos() ? mark.BufferIndex : CurrentPos();
                Array.Clear(_buffer, startPos, Math.Abs(CurrentPos() - mark.BufferIndex));
            });
        }

        private void MoveToMark(Func<Option<int>> getMarkIndex) {
            getMarkIndex().Apply(index => {
                var mark = _marks[index];
                SetCursorPos(mark.CursorX, mark.CursorY);
            });
        }

        private T[] GetToMark(Func<Option<int>> getMarkIndex) {
            var maybeIndex = getMarkIndex();
            if (maybeIndex.IsNone) return new T[0];

            var mark = _marks[maybeIndex.Value];
            var startPos = mark.BufferIndex < CurrentPos() ? mark.BufferIndex : CurrentPos();
            var result = new T[Math.Abs(CurrentPos() - mark.BufferIndex)];
            Array.Copy(_buffer, startPos, result, 0, result.Length);
            return result;
        }

        private bool RemoveMark(Func<Option<int>> getMarkIndex) {
            return getMarkIndex().Apply(index => {
                _marks.Remove(_marks[index]);
                return true;
            }).IsSome;
        }
        
        private Option<int> GetNextMarkIndex() {
            if (_marks.Count == 0) return new None();
            var index = _marks.FindInsertionIndex(CurrentPos());
            return index >= _marks.Count ? new None() : new Option<int>(index);
        }

        private Option<int> GetPreviousMarkIndex() {
            if (_marks.Count == 0) return new None();
            var index = _marks.FindInsertionIndex(CurrentPos()) - (IsOnMark() ? 2 : 1);
            return index < 0 ? new None() : new Option<int>(index);
        }

        private Option<int> GetCurrentMarkIndex() {
            if (_marks.Count == 0) return new None();
            var index = _marks.FindInsertionIndex(CurrentPos()) - 1;
            if (index == _marks.Count) return new None();
            return _marks[index].BufferIndex == CurrentPos() ? new Option<int>(index) : new None();
        }

        private int CurrentPos() {
            return CursorY.Value * Width.Value + CursorX.Value;
        }

        private void RaisePosChanged() {
            PositionChanged?.Invoke(this, Tuple.Create(CursorX.Value, CursorY.Value));
        }

        private Tuple<int, int> GetPosFromIndex(int index) {
            var y = index / Width.Value;
            var x = index - (y * Width.Value);
            return Tuple.Create(x, y);
        }
    }
}
