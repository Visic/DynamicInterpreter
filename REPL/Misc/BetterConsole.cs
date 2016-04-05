using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    //TODO:: Parameterize move functions to allow stopping movement at the Mark instead of just the buffer bounds
    public static class BetterConsole {
        static NavigableBuffer<char> _buffer = new NavigableBuffer<char>();

        static BetterConsole() {
            _buffer.Width.Value = Console.WindowWidth;
            _buffer.Height.Value = Console.BufferHeight;
            _buffer.ValueWritten += (s, v) => Console.Write(new string(v));
            _buffer.LineCleared += (s, v) => {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(Enumerable.Repeat(' ', Console.BufferWidth - 1).ToArray()));
                Console.SetCursorPosition(0, Console.CursorTop);
            };
            _buffer.PositionChanged += (s, pos) => Console.SetCursorPosition(pos.Item1, pos.Item2);
        }

        public static int CursorX {
            get { return _buffer.CursorX.Value; }
            set { _buffer.CursorX.Value = value; }
        }

        public static int CursorY {
            get { return _buffer.CursorY.Value; }
            set { _buffer.CursorY.Value = value; }
        }

        public static void Write(string val) {
            Write(val.Split('\n'));
        }

        public static void Write(params string[] lines) {
            Methods.For(lines, (i, line) => {
                _buffer.Write(line);
                if (lines.Length > 1 && i < lines.Length - 1) SetCursorPos(0, CursorY + 1);
            });
        }

        public static void Write(string fmtstr, params object[] args) {
            Write(string.Format(fmtstr, args));
        }

        public static void Write(object val) {
            Write(val.ToString());
        }

        public static void WriteOnNextLine(string fmtstr, params object[] args) {
            MoveToNextLine();
            Write(fmtstr, args);
        }

        public static void WriteOnNextLine(string val) {
            MoveToNextLine();
            Write(val);
        }

        public static void WriteOnNextLine(object val) {
            MoveToNextLine();
            Write(val);
        }

        public static void WriteLine(object val) {
            Write(val);
            SetCursorPos(0, CursorY + 1);
        }

        public static void WriteLine(string fmtstr, params object[] args) {
            Write(fmtstr, args);
            SetCursorPos(0, CursorY + 1);
        }

        public static void Backspace() {
            if(_buffer.MoveLeft()) _buffer.Write(false, ' ');
        }

        public static void ClearLine() {
            _buffer.ClearLine();
        }

        public static bool MoveLeft() {
            return _buffer.MoveLeft(true);
        }

        public static bool MoveRight() {
            return _buffer.MoveRight(true);
        }

        public static bool MoveUp() {
            return _buffer.MoveUp(true);
        }

        public static bool MoveDown() {
            return _buffer.MoveDown(true);
        }

        public static bool MoveToNextLine() {
            return _buffer.SetCursorPos(0, _buffer.CursorY.Value + 1);
        }

        public static bool SetCursorPos(int x, int y) {
            return _buffer.SetCursorPos(x, y);
        }

        public static void MarkPos() {
            _buffer.MarkPos();
        }

        public static bool IsOnMArk() {
            return _buffer.IsOnMark();
        }

        public static void MoveToNextMark() {
            _buffer.MoveToNextMark();
        }

        public static void MoveToPreviousMark() {
            _buffer.MoveToPreviousMark();
        }

        public static void RemoveAllMarks() {
            _buffer.RemoveAllMarks();
        }

        public static void RemoveCurrentMark() {
            _buffer.RemoveCurrentMark();
        }

        public static void RemoveNextMark() {
            _buffer.RemoveNextMark();
        }

        public static void RemovePreviousMark() {
            _buffer.RemovePreviousMark();
        }

        public static string GetToNextMark() {
            return new string(_buffer.GetToNextMark()).Trim('\0');
        }

        public static string GetToPreviousMark() {
            return new string(_buffer.GetToPreviousMark()).Trim('\0');
        }

        public static void ClearToNextMark() {
            _buffer.VisitToNextMark((pos, prevVal, curVal) => {
                _buffer.Write(false, ' ');
            });
        }

        public static void ClearToPreviousMark() {
            _buffer.VisitToPreviousMark((pos, prevVal, curVal) => {
                _buffer.Write(false, ' ');
            });
        }
    }
}
