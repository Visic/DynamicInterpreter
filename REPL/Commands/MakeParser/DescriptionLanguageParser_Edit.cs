//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;
using REPL.MakeParser;

namespace DynamicInterpreter {
    public static partial class DescriptionLanguageParser {
        static ISymbolHandler[] _symbolHandlers = new ISymbolHandler[] {
            //////ADD HANDLERS HERE//////
            new LiteralHandler(), new SymbolHandler(), new AnyCharHandler(),
            new RangeHandler(), new FallbackPointHandler(), new InOrderHandler(),
            new AnyHandler(), new NegationHandler(), new AssignmentHandler(),
            new AllAssignmentsHandler(), new EntryPointHandler(), new GroupHandler(),
            new IgnoreWhitespaceHandler(), new AllCharsNotGTHandler(), new AllCharsNotQuoteHandler()
            //////ADD HANDLERS HERE//////
        };

        #region Handlers
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

        public class AnyCharHandler : ISymbolHandler {
            public string SymbolName { get; } = "anychar";

            public List<object> Call(List<object> args) {
                return new List<object> { ParserCodeGenerator.AnyChar() };
            }
        }

        public class RangeHandler : ISymbolHandler {
            public string SymbolName { get; } = "range";

            public List<object> Call(List<object> args) {
                return new List<object> { ParserCodeGenerator.Range(((string)args[1])[0], ((string)args[3])[0]) };
            }
        }

        public class FallbackPointHandler : ISymbolHandler {
            public string SymbolName { get; } = "fallback_point";

            public List<object> Call(List<object> args) {
                var assignment = (Tuple<string, string>)args[1];
                return new List<object> { Tuple.Create(assignment.Item1, ParserCodeGenerator.FallbackPoint(assignment.Item2)) };
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
                var newSymbolName = (string)args[1];
                return new List<object> { Tuple.Create(newSymbolName, ParserCodeGenerator.Assignment(newSymbolName, (string)args[4])) };
            }
        }

        public class AllAssignmentsHandler : ISymbolHandler {
            public string SymbolName { get; } = "all_all_assignments";

            public List<object> Call(List<object> args) {
                var castArgs = args.Take(args.Count - 1).Cast<Tuple<string, string>>().ToArray();
                return ParserCodeGenerator.AllAssignments(castArgs).Cast<object>().ToList();
            }
        }

        public class EntryPointHandler : ISymbolHandler {
            public string SymbolName { get; } = "EntryPoint";

            public List<object> Call(List<object> args) {
                return ParserCodeGenerator.EntryPoint(args.Cast<string>()).Cast<object>().ToList();
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
        #endregion
    }
}