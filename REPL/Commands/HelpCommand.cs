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
        public override string CommandInfo { get; } = "This command will list general information and usage of any command.";
        public override string Usage { get; } = "Usage: Help {command name}";
        public override int RequiredArgNum { get; } = 0;

        public override void Help() {
            base.Help();
            _commands[Constants.ListCmdName.ToLower()].Call(new List<string>());
        }

        protected override void ExecuteCmd(List<string> args) {
            if(args.Count == 0) {
                Help();
            } else {
                var maybeCmd = _commands.TryGetValue((args[0]).ToLower());
                if (maybeCmd.IsNone) {
                    BetterConsole.WriteOnNextLine(Constants.UnknownCmdFmtStr, args[0]);
                } else {
                    maybeCmd.Value.Help();
                }
            }
        }
    }
}
