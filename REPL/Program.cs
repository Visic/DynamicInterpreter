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
            var done = false;
            while(!done) {
                BetterConsole.Prompt(
                    $"{promptText}>", 
                    BetterConsole.MakeKeyHandler(ConsoleKey.Escape, v => done = true),
                    BetterConsole.MakeKeyHandler(ConsoleKey.Enter, v => { handler(v); BetterConsole.WriteLine(); return true; })
                );
            }
        }
    }
}