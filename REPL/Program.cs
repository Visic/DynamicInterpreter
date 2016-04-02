using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public class Program {
        public enum Prompt { Commands, Repl, Help };
        static Prompt _activePrompt;
        static string _languageName = "";

        public static void Main(string[] args) {
            while (true) {
                REPLPrompt();
                LanguagePrompt();
            }
        }

        static void REPLPrompt() {
            RunPrompt(Constants.ReplPromptName, ConsoleKey.Escape);
        }

        static void LanguagePrompt() {
            RunPrompt(_languageName, ConsoleKey.Escape);
        }

        static void RunPrompt(string promptName, Union<char, ConsoleKey> breakKey) {
            string currentText = "";
            ClearLine();
            Console.Write($"{promptName}>");
            while (true) {
                var key = Console.ReadKey();
                if (breakKey.Match<bool>(x => key.KeyChar == x, x => key.Key == x)) {
                    break;
                } else if (key.Key == ConsoleKey.Enter) {
                    Console.Write($"\n{promptName}>");
                } else {
                    currentText += key.KeyChar;
                }
            }
        }

        static void ClearLine() {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(Enumerable.Repeat(' ', Console.BufferWidth - 1).ToArray()));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
