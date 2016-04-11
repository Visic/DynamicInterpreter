using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    public class ListCommand : REPLCommand {
        Dictionary<string, REPLCommand> _commands;
        public ListCommand(Dictionary<string, REPLCommand> commands) { _commands = commands; }

        public override string CommandInfo { get; } = "This command prints a list of all available REPL commands.";
        public override string Name { get; } = Constants.ListCmdName;
        public override int RequiredArgNum { get; } = 0;

        protected override void ExecuteCmd(List<string> args) {
            BetterConsole.WriteOnNextLine($"Available commands: {string.Join(", ", _commands.Values.Select(x => x.Name).ToArray())}");
        }
    }
}
