using System.Linq;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public class CombineToStringSymbolHandler : GenericSymbolHandler {
        public CombineToStringSymbolHandler(string name) : base(name, x => new List<object>() { x.ToDelimitedString("") }) {}
    }
}
