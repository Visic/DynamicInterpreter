using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicInterpreter;
using Utility;

namespace ParserGraphing {
    public static class GraphHelper {
        const string C_FilePath = "graph.txt";

        public static void AddNode(string nodeName) {
            File.AppendAllText(C_FilePath, $"\nnode {nodeName}");
        }

        public static void AddEdge(string nodeName1, string nodeName2) {
            File.AppendAllText(C_FilePath, $"\nedge {nodeName1},{nodeName2}");
        }
    }

    public class LiteralHandler : ISymbolHandler {
        public string SymbolName { get; } = "literal";

        public List<object> Call(List<object> args) {
            var nodeName = (string)args[1];
            GraphHelper.AddNode(nodeName);
            return new List<object> { nodeName };
        }
    }

    public class SymbolHandler : ISymbolHandler {
        public string SymbolName { get; } = "symbol";

        public List<object> Call(List<object> args) {
            var nodeName = (string)args[1];
            GraphHelper.AddNode(nodeName);
            return new List<object> { nodeName };
        }
    }

    public class AnyCharHandler : ISymbolHandler {
        public string SymbolName { get; } = "anychar";

        public List<object> Call(List<object> args) {
            GraphHelper.AddNode("*");
            return new List<object> { "*" };
        }
    }

    public class RangeHandler : ISymbolHandler {
        public string SymbolName { get; } = "range";

        public List<object> Call(List<object> args) {
            var nodeName = $"[{((string)args[1])[0]} - {((string)args[3])[0]}]";
            GraphHelper.AddNode(nodeName);
            return new List<object> { nodeName };
        }
    }

    //public class FallbackPointHandler : ISymbolHandler {
    //    public string SymbolName { get; } = "fallback_point";

    //    public List<object> Call(List<object> args) {
    //        var assignment = (Tuple<string, string>)args[1];
    //        return new List<object> { Tuple.Create(assignment.Item1, ParserCodeGenerator.FallbackPoint(assignment.Item2)) };
    //    }
    //}

    public class InOrderHandler : ISymbolHandler {
        public string SymbolName { get; } = "all_inorder";

        public List<object> Call(List<object> args) {
            for(int i = 0; i < args.Count - 1; ++i) {
                GraphHelper.AddEdge((string)args[i], (string)args[i + 1]);
            }
            return new List<object> { };
        }
    }

    //public class AnyHandler : ISymbolHandler {
    //    public string SymbolName { get; } = "all_any";

    //    public List<object> Call(List<object> args) {
    //        if(args.Count == 1) return args;
    //        return new List<object> { ParserCodeGenerator.Any(args.Where(x => (string)x != "|").Cast<string>()) };
    //    }
    //}

    //public class NegationHandler : ISymbolHandler {
    //    public string SymbolName { get; } = "negation";

    //    public List<object> Call(List<object> args) {
    //        return new List<object> { ParserCodeGenerator.Negate((string)args[1]) };
    //    }
    //}

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
