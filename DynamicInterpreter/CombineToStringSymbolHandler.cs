using System.Linq;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public class CombineToStringSymbolHandler : GenericSymbolHandler {
        public CombineToStringSymbolHandler(string name) : base(name, x => new List<Option<object>>() { x.Select(y => y.Value).ToDelimitedString("") }) {}
    }
}
