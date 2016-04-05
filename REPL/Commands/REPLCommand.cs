using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    public abstract class REPLCommand {
        public abstract string Name { get; }
        public abstract void Call(List<object> args);
        public abstract void Help();
    }
}
