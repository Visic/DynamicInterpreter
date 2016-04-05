using DynamicInterpreter;
using REPL.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public class Program {
        static Dictionary<string, REPLCommand> _replCommands = new Dictionary<string, REPLCommand>();
        static Interpreter _commandInterp = new Interpreter();
        static string _languageName = "";

        public static void Main(string[] args) {
            SetupREPLCommands();
            var commandHandler = new GenericSymbolHandler("command", CallREPLCommand);
            _commandInterp.Setup(Resources.REPLCommandGrammar, new IgnoreSymbolHandler("whitespace"), new CombineToStringSymbolHandler("arg"), new CombineToStringSymbolHandler("cmdname"), commandHandler);

            while(true) { //handle switching prompts
                Prompt(Constants.ReplPromptName, x => { _commandInterp.Execute(x); });
                Prompt(_languageName, x => { });
            }
        }

        private static void SetupREPLCommands() {
            Action<REPLCommand> addCmd = x => _replCommands[x.Name.ToLower()] = x;
            addCmd(new HelpCommand(_replCommands));
            addCmd(new ListCommand(_replCommands));
        }

        private static List<object> CallREPLCommand(List<object> args) {
            var maybeCmd = _replCommands.TryGetValue(((string)args[0]).ToLower());
            if(maybeCmd.IsNone) BetterConsole.WriteOnNextLine(Constants.UnknownCmdFmtStr, args[0]);
            else maybeCmd.Value.Call(args.Skip(1).ToList());
            return new List<object>();
        }

        private static void Prompt(string promptText, Action<string> handler) {
            BetterConsole.ClearLine();
            BetterConsole.Write($"{promptText}>");
            BetterConsole.MarkPos();
            while(true) {
                var key = Console.ReadKey(true);
                if(key.Key == ConsoleKey.Escape) break; //switch prompts

                if(key.Key == ConsoleKey.Enter) {
                    handler(BetterConsole.GetToPreviousMark());
                    BetterConsole.RemoveAllMarks();
                    BetterConsole.WriteOnNextLine($"{promptText}>");
                    BetterConsole.MarkPos();
                } else if(key.Key == ConsoleKey.LeftArrow) {
                    BetterConsole.MoveLeft();
                } else if(key.Key == ConsoleKey.RightArrow) {
                    BetterConsole.MoveRight();
                } else if(key.Key == ConsoleKey.UpArrow) {
                    BetterConsole.MoveUp();
                } else if(key.Key == ConsoleKey.DownArrow) {
                    if (!BetterConsole.MoveDown()) {
                        BetterConsole.MoveToNextMark();
                        BetterConsole.ClearToPreviousMark();
                        BetterConsole.RemoveNextMark();
                    }
                } else if(key.Key == ConsoleKey.Backspace) {
                    BetterConsole.Backspace();
                    BetterConsole.RemoveNextMark();
                    BetterConsole.MarkPos();
                } else {
                    var textAfterPrompt = BetterConsole.GetToPreviousMark().Length > 0;
                    BetterConsole.Write(key.KeyChar);
                    if (textAfterPrompt) BetterConsole.RemovePreviousMark(); //TODO:: Handle insertion
                    BetterConsole.MarkPos();
                }
            }
        }
    }
}