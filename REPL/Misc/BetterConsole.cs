using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public class BetterConsole {
        private BetterConsole() { }

        public struct Key {
            int _hashcode;
            public Key(ConsoleKey key, ConsoleModifiers keyModifiers = 0) {
                ConsoleKey = key;
                KeyModifiers = keyModifiers;
                _hashcode = int.Parse($"{(int)ConsoleKey}{KeyModifiers}");
            }

            public readonly ConsoleKey ConsoleKey;
            public readonly ConsoleModifiers KeyModifiers;

            public static implicit operator Key(ConsoleKey consoleKey) {
                return new Key(consoleKey);
            }

            #region equals
            public override bool Equals(object obj) {
                if(obj is Key) return (Key)obj == this;
                if(obj is ConsoleKeyInfo) return (ConsoleKeyInfo)obj == this;
                return false;
            }

            public override int GetHashCode() {
                return _hashcode;
            }

            public static bool operator ==(Key key1, Key key2) {
                return key1.ConsoleKey == key2.ConsoleKey && key1.KeyModifiers == key2.KeyModifiers;
            }

            public static bool operator !=(Key key, Key keyInfo) {
                return !(key == keyInfo);
            }

            public static bool operator ==(Key key, ConsoleKeyInfo keyInfo) {
                return key.ConsoleKey == keyInfo.Key && key.KeyModifiers == keyInfo.Modifiers;
            }

            public static bool operator !=(Key key, ConsoleKeyInfo keyInfo) {
                return !(key == keyInfo);
            }

            public static bool operator ==(ConsoleKeyInfo keyInfo, Key key) {
                return key.ConsoleKey == keyInfo.Key && key.KeyModifiers == keyInfo.Modifiers;
            }

            public static bool operator !=(ConsoleKeyInfo keyInfo, Key key) {
                return !(key == keyInfo);
            }
            #endregion
        }

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

        public static int CursorX
        {
            get { return _buffer.CursorX.Value; }
            set { _buffer.CursorX.Value = value; }
        }

        public static int CursorY
        {
            get { return _buffer.CursorY.Value; }
            set { _buffer.CursorY.Value = value; }
        }

        public static Tuple<Key, Func<string, bool>> MakeKeyHandler(Key key, Func<string, bool> handler) {
            return Tuple.Create(key, handler);
        }

        public static void Prompt(string prompt, IReadOnlyList<string> prevCmds, params Tuple<Key, Func<string, bool>>[] keyHandlers) {
            Func<string> getCurrentText = () => {
                var curX = CursorX;
                var curY = CursorY;
                _buffer.MoveToNextMark();
                var result = GetToPreviousMark();
                CursorX = curX;
                CursorY = curY;
                return result;
            };

            var index = prevCmds.Count;
            _buffer.ClearLine();
            Write(prompt);
            _buffer.MarkPos();
            _buffer.MarkPos();
            while(true) {
                var key = Console.ReadKey(true);
                var shouldBreak = keyHandlers.Aggregate(false, (acc, val) => val.Item1 == key ? acc || val.Item2(getCurrentText()) : acc);
                if(shouldBreak) break;

                if(key.Key == ConsoleKey.LeftArrow) {
                    _buffer.MoveLeft(true);
                } else if(key.Key == ConsoleKey.RightArrow) {
                    _buffer.MoveRight(true);
                } else if(key.Key == ConsoleKey.UpArrow) {
                    if(!_buffer.MoveUp(true)) {
                        RemoveCurrentPromptInput();
                        if(index >= 0) --index;
                        if(index >= 0 && prevCmds.Count > 0) Write(prevCmds[index]);
                    }
                } else if(key.Key == ConsoleKey.DownArrow) {
                    if(!_buffer.MoveDown(true)) {
                        RemoveCurrentPromptInput();
                        if(index < prevCmds.Count) ++index;
                        if(index < prevCmds.Count && prevCmds.Count > 0) Write(prevCmds[index]);
                    }
                } else if(key.Key == ConsoleKey.Backspace) {
                    RemoveNearbyChar(true);
                } else if(key.Key == ConsoleKey.Delete) {
                    RemoveNearbyChar(false);
                } else {
                    if(key.KeyChar == '\0') continue;
                    ShiftRest(false);
                    Write(key.KeyChar);
                }
            }
            _buffer.RemoveAllMarks();
        }

        public static void Write(string val) {
            Write(val.Split('\n'));
        }

        public static void Write(params string[] lines) {
            Methods.For(lines, (i, line) => {
                _buffer.Write(line);
                if(lines.Length > 1 && i < lines.Length - 1) _buffer.SetCursorPos(0, CursorY + 1);
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
            _buffer.SetCursorPos(0, CursorY + 1);
        }

        public static void WriteLine() {
            _buffer.SetCursorPos(0, CursorY + 1);
        }

        public static void WriteLine(string fmtstr, params object[] args) {
            Write(fmtstr, args);
            _buffer.SetCursorPos(0, CursorY + 1);
        }

        private static void RemoveNearbyChar(bool left) {
            if(left && !_buffer.MoveLeft(true)) return;
            ShiftRest(true);
        }

        private static void ShiftRest(bool left) {
            var rest = _buffer.GetToNextMark().ToList();
            rest = left ? rest.Add_Ex('\0').Skip(1).ToList() : rest.Insert_Ex(0, '\0');
            if(rest.Count == 0) return;
            _buffer.MarkPos();
            _buffer.Write(rest, true);

            if(left) {
                _buffer.RemoveCurrentMark();
                _buffer.MoveLeft(true);
            } else {
                _buffer.RemovePreviousMark();
            }

            _buffer.MarkPos();
            _buffer.MoveToPreviousMark();
            _buffer.RemoveCurrentMark();
        }

        private static bool MoveToNextLine() {
            return _buffer.SetCursorPos(0, _buffer.CursorY.Value + 1);
        }

        private static string GetToPreviousMark() {
            return new string(_buffer.GetToPreviousMark()).Trim('\0');
        }

        private static void RemoveCurrentPromptInput() {
            _buffer.MoveToNextMark();
            ClearToPreviousMark();
            _buffer.RemoveNextMark();
            _buffer.MarkPos();
        }

        private static void ClearToPreviousMark() {
            _buffer.VisitToPreviousMark((pos, prevVal, curVal) => {
                _buffer.Write(false, ' ');
            });
        }
    }
}
