using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
