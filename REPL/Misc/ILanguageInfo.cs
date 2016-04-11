using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    public interface ILanguageInfo {
        string Name { get; }
        string Grammar { get; }
    }
}
