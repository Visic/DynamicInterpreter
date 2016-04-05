using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public class HelpCommand : REPLCommand {
        Dictionary<string, REPLCommand> _commands;
        public HelpCommand(Dictionary<string, REPLCommand> commands) { _commands = commands; }

        public override string Name { get; } = Constants.HelpCmdName;

        public override void Call(List<object> args) {
            if(args.Count == 0) {
                Help();
            } else {
                var maybeCmd = _commands.TryGetValue(((string)args[0]).ToLower());
                if (maybeCmd.IsNone) {
                    BetterConsole.WriteOnNextLine(Constants.UnknownCmdFmtStr, args[0]);
                } else {
                    maybeCmd.Value.Help();
                }
            }
        }

        public override void Help() {
            BetterConsole.WriteOnNextLine("Usage: Help {command name}");
            _commands[Constants.ListCmdName.ToLower()].Call(new List<object>());
        }
    }
}
