using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    public class ListCommand : REPLCommand {
        Dictionary<string, REPLCommand> _commands;
        public ListCommand(Dictionary<string, REPLCommand> commands) { _commands = commands; }

        public override string Name { get; } = Constants.ListCmdName;

        public override void Call(List<object> args) {
            BetterConsole.WriteOnNextLine($"Available commands: {string.Join(", ", _commands.Values.Select(x => x.Name).ToArray())}");
        }

        public override void Help() {
            BetterConsole.WriteOnNextLine("This command prints a list of all available REPL commands.");
        }
    }
}
