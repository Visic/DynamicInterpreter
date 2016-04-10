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
        static List<string> _cmdBuffer = new List<string>();
        static List<string> _langBuffer = new List<string>();

        public static void Main(string[] args) {
            SetupREPLCommands();
            var commandHandler = new GenericSymbolHandler("command", CallREPLCommand);
            _commandInterp.Setup(Resources.REPLCommandGrammar, new IgnoreSymbolHandler("whitespace"), new CombineToStringSymbolHandler("arg"), new CombineToStringSymbolHandler("cmdname"), commandHandler);

            while(true) { //handle switching prompts
                Prompt(Constants.ReplPromptName, x => { _commandInterp.Execute(x); }, _cmdBuffer);
                Prompt(_languageName, x => { }, _langBuffer);
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

        private static void Prompt(string promptText, Action<string> handler, List<string> buffer) {
            var done = false;
            while(!done) {
                BetterConsole.Prompt(
                    $"{promptText}>", 
                    buffer,
                    BetterConsole.MakeKeyHandler(ConsoleKey.Escape, v => done = true),
                    BetterConsole.MakeKeyHandler(ConsoleKey.Enter, v => {
                        buffer.Remove(v);
                        buffer.Add(v);

                        handler(v);
                        BetterConsole.WriteLine();
                        return true;
                    })
                );
            }
        }
    }
}