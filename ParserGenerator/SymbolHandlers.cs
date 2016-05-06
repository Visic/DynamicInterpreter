using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicInterpreter;
using Utility;

namespace ParserGenerator {
    public class LiteralHandler : ISymbolHandler {
        public string SymbolName { get; } = "literal";

        public List<object> Call(List<object> args) {
            return new List<object> { ParserCodeGenerator.Literal(((string)args[1]).Replace("\"", "\\\"")) };
        }
    }

    public class SymbolHandler : ISymbolHandler {
        public string SymbolName { get; } = "symbol";

        public List<object> Call(List<object> args) {
            return new List<object> { ParserCodeGenerator.Symbol(((string)args[1]).Replace("\"", "\\\"")) };
        }
    }

    public class InOrderHandler : ISymbolHandler {
        public string SymbolName { get; } = "all_inorder";

        public List<object> Call(List<object> args) {
            if(args.Count == 1) return args;
            return new List<object> { ParserCodeGenerator.InOrder(args.Cast<string>()) };
        }
    }

    public class AnyHandler : ISymbolHandler {
        public string SymbolName { get; } = "all_any";

        public List<object> Call(List<object> args) {
            if(args.Count == 1) return args;
            return new List<object> { ParserCodeGenerator.Any(args.Where(x => (string)x != "|").Cast<string>()) };
        }
    }

    public class NegationHandler : ISymbolHandler {
        public string SymbolName { get; } = "negation";

        public List<object> Call(List<object> args) {
            return new List<object> { ParserCodeGenerator.Negate((string)args[1]) };
        }
    }

    public class AssignmentHandler : ISymbolHandler {
        public string SymbolName { get; } = "assignment";

        public List<object> Call(List<object> args) {
            return new List<object> { ParserCodeGenerator.Assignment((string)args[1], (string)args[4]) };
        }
    }

    public class EntryPointHandler : ISymbolHandler {
        public string SymbolName { get; } = "EntryPoint";

        public List<object> Call(List<object> args) {
            File.WriteAllText("Parser.cs", ParserCodeGenerator.EntryPoint(args.Cast<string>()));
            return new List<object>();
        }
    }

    public class GroupHandler : ISymbolHandler {
        public string SymbolName { get; } = "group";

        public List<object> Call(List<object> args) {
            return new List<object> { args[1] }; //just return the "any" parser
        }
    }

    public class IgnoreWhitespaceHandler : IgnoreSymbolHandler {
        public IgnoreWhitespaceHandler() : base("ignore_all_whitespace") { }
    }

    public class AllCharsNotGTHandler : ISymbolHandler {
        public string SymbolName { get; } = "allchars_not_gt";

        public List<object> Call(List<object> args) {
            return new List<object> { args.Cast<string>().ToDelimitedString("").Replace(@"\>", ">") };
        }
    }

    public class AllCharsNotQuoteHandler : ISymbolHandler {
        public string SymbolName { get; } = "allchars_not_quote";

        public List<object> Call(List<object> args) {
            return new List<object> { args.Cast<string>().ToDelimitedString("").Replace(@"\'", "'") };
        }
    }
}
