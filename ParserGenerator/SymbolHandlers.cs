using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicInterpreter;

namespace ParserGenerator {
    public class LiteralHandler : ISymbolHandler {
        public string SymbolName { get; } = "literal";

        public List<object> Call(List<object> args) {
            var str = (string)args[1];
            str = str.Replace(@"\'", "'");
            str = str.Replace("\"", "\\\"");

            return new List<object> { ParserCodeGenerator.Literal(str) };
        }
    }

    public class SymbolHandler : ISymbolHandler {
        public string SymbolName { get; } = "symbol";

        public List<object> Call(List<object> args) {
            return new List<object>() { ParserCodeGenerator.Symbol((string)args[1]) };
        }
    }

    public class InOrderHandler : ISymbolHandler {
        public string SymbolName { get; } = "all_inorder";

        public List<object> Call(List<object> args) {
            if(args.Count == 1) return args;
            return new List<object>() { ParserCodeGenerator.InOrder(args.Cast<string>()) };
        }
    }

    public class AnyHandler : ISymbolHandler {
        public string SymbolName { get; } = "all_any";

        public List<object> Call(List<object> args) {
            if(args.Count == 1) return args;
            return new List<object>() { ParserCodeGenerator.Any(args.Where(x => (string)x != "|").Cast<string>()) };
        }
    }

    public class NegationHandler : ISymbolHandler {
        public string SymbolName { get; } = "negation";

        public List<object> Call(List<object> args) {
            return new List<object>() { ParserCodeGenerator.Negate((string)args[1]) };
        }
    }

    public class AssignmentHandler : ISymbolHandler {
        public string SymbolName { get; } = "assignment";

        public List<object> Call(List<object> args) {
            return new List<object>() { ParserCodeGenerator.Assignment((string)args[1], (string)args[4]) };
        }
    }

    public class EntryPointHandler : ISymbolHandler {
        public string SymbolName { get; } = "EntryPoint";

        public List<object> Call(List<object> args) {
            File.WriteAllText("parser.cs", ParserCodeGenerator.EntryPoint(args.Cast<string>()));
            return new List<object>();
            //return new List<object>() { ParserCodeGenerator.EntryPoint(args.Cast<string>()) };
        }
    }

    public class GroupHandler : ISymbolHandler {
        public string SymbolName { get; } = "group";

        public List<object> Call(List<object> args) {
            return new List<object>() { args[1] }; //just return the "any" parser
        }
    }

    public class IgnoreWhitespaceHandler : IgnoreSymbolHandler {
        public IgnoreWhitespaceHandler() : base("ignore_all_whitespace") { }
    }

    public class AllCharsNotGTHandler : CombineToStringSymbolHandler {
        public AllCharsNotGTHandler() : base("allchars_not_gt") { }
    }

    public class AllCharsNotQuoteHandler : CombineToStringSymbolHandler {
        public AllCharsNotQuoteHandler() : base("allchars_not_quote") { }
    }
}
