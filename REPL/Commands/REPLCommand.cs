using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    public abstract class REPLCommand {
        public abstract string Name { get; }
        public abstract string CommandInfo { get; }
        public virtual string Usage { get; }
        public abstract int RequiredArgNum { get; }

        public virtual void Help() {
            BetterConsole.WriteOnNextLine(CommandInfo);
            if(!string.IsNullOrEmpty(Usage)) BetterConsole.WriteOnNextLine(Usage);
        }

        public void Call(List<string> args) {
            if(args.Count < RequiredArgNum) {
                if(!string.IsNullOrEmpty(Usage)) BetterConsole.WriteOnNextLine(Usage);
            } else {
                ExecuteCmd(args);
            }
        }

        protected abstract void ExecuteCmd(List<string> args);
    }
}
