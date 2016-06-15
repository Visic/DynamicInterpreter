using System.Collections.Generic;
using DynamicInterpreter;

namespace TestLanguage {
    public class AddHandler : ISymbolHandler {
        public string SymbolName { get; } = "add";

        public List<object> Call(List<object> args) {
            return new List<object>() { int.Parse(args[0].ToString()) + int.Parse(args[2].ToString()) };
        }
    }

    public class IgnoreWhitespaceHandler : IgnoreSymbolHandler {
        public IgnoreWhitespaceHandler() : base("ignore_whitespace") { }
    }

    public class IntegerHandler : CombineToStringSymbolHandler {
        public IntegerHandler() : base("integer") { }
    }
}
