using System;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public class GenericSymbolHandler : ISymbolHandler {
        Func<List<object>, List<object>> _call;
        public GenericSymbolHandler(string name, Func<List<object>, List<object>> call) {
            SymbolName = name;
            _call = call;
        }

        public string SymbolName { get; }
        public List<object> Call(List<object> args) { return _call(args); }
    }
}
