using System;
using System.Collections.Generic;
using DynamicInterpreter;
using System.Linq;

namespace TestLanguage {
    public class AddHandler : ISymbolHandler {
        public string SymbolName { get; } = "add";

        public List<object> Call(List<object> args) {
            return new List<object>() { (int)args[0] + (int)args[4] };
        }
    }

    public class WhitespaceHandler : CombineToStringSymbolHandler {
        public WhitespaceHandler() : base("whitespace") { }
    }

    public class IntegerHandler : ISymbolHandler {
        public string SymbolName { get; } = "integer";

        public List<object> Call(List<object> args) {
            int value;
            var intStr = string.Join("", args.Cast<string>().ToArray());
            if (!int.TryParse(intStr, out value)) throw new Exception(string.Format("{0} is not between -2^31 and 2^31 - 1", intStr));

            return new List<object> { value };
        }
    }
}
