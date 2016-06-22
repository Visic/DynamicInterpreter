using System;
using System.Collections.Generic;
using DynamicInterpreter;
using System.Linq;

namespace TestLanguage {
    public class AddHandler : ISymbolHandler {
        public string SymbolName { get; } = "add";

        public List<object> Call(int characterIndex, List<object> args) {
            int left, right;
            var errStrFmt = "{0} is not between -2^31 and 2^31 - 1";
            if (!int.TryParse(args[0].ToString(), out left)) return new List<object>() { new Error(string.Format(errStrFmt, args[0]), characterIndex) };
            if(!int.TryParse(args[4].ToString(), out right)) return new List<object>() { new Error(string.Format(errStrFmt, args[4]), characterIndex) };

            return new List<object>() { left + right };
        }
    }

    public class WhitespaceHandler : CombineToStringSymbolHandler {
        public WhitespaceHandler() : base("whitespace") { }
    }

    public class IntegerHandler : ISymbolHandler {
        public string SymbolName { get; } = "integer";

        public List<object> Call(int characterIndex, List<object> args) {
            return new List<object>() { Tuple.Create(characterIndex, string.Join("", args.Cast<string>().ToArray())) };
        }
    }
}
