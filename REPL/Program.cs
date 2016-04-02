using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public class Program {
        static string _languageName = "";

        public static void Main(string[] args) {
            while (true) {
                RunPrompt(Constants.ReplPromptName);
                RunPrompt(_languageName);
            }
        }

        static void RunPrompt(string promptName) {
            string currentText = "";
            ClearLine();
            Console.Write($"{promptName}>");
            while (true) {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) break;

                if (key.Key == ConsoleKey.Enter) {
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
