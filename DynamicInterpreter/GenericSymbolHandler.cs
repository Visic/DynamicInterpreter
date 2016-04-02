using System;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public class GenericSymbolHandler : ISymbolHandler {
        Func<List<Option<object>>, List<Option<object>>> _call;
        public GenericSymbolHandler(string name, Func<List<Option<object>>, List<Option<object>>> call) {
            SymbolName = name;
            _call = call;
        }

        public string SymbolName { get; }
        public List<Option<object>> Call(List<Option<object>> args) { return _call(args); }
    }
}
