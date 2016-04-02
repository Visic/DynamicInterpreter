using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public class IgnoreSymbolHandler : GenericSymbolHandler {
        public IgnoreSymbolHandler(string name) : base(name, x => new List<Option<object>>()) {}
    }
}
