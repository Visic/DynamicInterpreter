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
        public static string LanguageName = "";
        public static Interpreter _languageInterp;

        static CommandDictionary _replCommands = new CommandDictionary();
        static Interpreter _commandInterp = new Interpreter();
        static List<string> _cmdBuffer = new List<string>();
        static List<string> _langBuffer = new List<string>();

        public static void Main(string[] args) {
            Console.Clear();

            var commandHandler = new GenericSymbolHandler("command", CallREPLCommand);
            _commandInterp.Setup(
                Parser.GenerateParser(Resources.REPLCommandGrammar).Match(x => x, x => { }).Value, //safe to ignore errors unless I decide to change the repl command parser definition
                new CombineToStringSymbolHandler("arg"),
                new CombineToStringSymbolHandler("cmdname"),
                commandHandler
            );

            while(true) { //handle switching prompts
                Prompt(Constants.ReplPromptName, x => { _commandInterp.Execute(x); }, _cmdBuffer);
                Prompt(LanguageName, x => {
                    if(_languageInterp != null) BetterConsole.WriteOnNextLine($"({_languageInterp.Execute(x).Match<string>(y => string.Join(", ", y.ToArray()), y => y)})");
                    else BetterConsole.WriteOnNextLine("Language not loaded. Use REPL Command {Language} to load a language assembly.");
                }, _langBuffer);
            }
        }

        private static List<object> CallREPLCommand(List<object> args) {
            var strArgs = args.Select(x => (string)x).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim('"')).ToList();
            var maybeCmd = _replCommands.TryGetValue(strArgs[0].ToLower());
            if(maybeCmd.IsNone) BetterConsole.WriteOnNextLine(Constants.UnknownCmdFmtStr, strArgs[0]);
            else maybeCmd.Value.Call(strArgs.Skip(1).ToList());
            return new List<object>();
        }

        private static void Prompt(string promptText, Action<string> handler, List<string> buffer) {
            var done = false;
            while(!done) {
                BetterConsole.Prompt(
                    $"{promptText}>", 
                    buffer,
                    BetterConsole.MakeKeyHandler(ConsoleKey.Escape, v => done = true),
                    BetterConsole.MakeKeyHandler(ConsoleKey.Enter, v => {
                        if(!string.IsNullOrEmpty(v)) {
                            buffer.Remove(v);
                            buffer.Add(v);
                            handler(v);
                        }

                        BetterConsole.WriteLine();
                        return true;
                    })
                );
            }
        }
    }
}