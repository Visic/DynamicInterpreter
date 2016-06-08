//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicInterpreter {
    public static partial class DescriptionLanguageInterpreter {
        static IReadOnlyDictionary<string, Parse> _symbolParsers = new Dictionary<string, Parse>() {
            {"whitespace", Parser.Symbol("whitespace", Parser.Any(Parser.Literal(" "), Parser.Literal("\t"), Parser.Literal("\r\n"), Parser.Literal("\n")))},
			{"escaped", Parser.Symbol("escaped", Parser.Any(Parser.InOrder(Parser.Literal("\\"), Parser.Negate(Parser.Literal("\\"))), Parser.InOrder(Parser.Literal("\\\\"), Parser.FixType(() => _symbolParsers["escaped"]))))},
			{"all_escape_chars", Parser.Symbol("all_escape_chars", Parser.Any(Parser.Literal("\\"), Parser.InOrder(Parser.Literal("\\"), Parser.FixType(() => _symbolParsers["all_escape_chars"]))))},
			{"almost_all_escape_chars", Parser.Symbol("almost_all_escape_chars", Parser.Any(Parser.InOrder(Parser.Literal("\\"), Parser.Negate(Parser.Negate(Parser.Literal("\\")))), Parser.InOrder(Parser.Literal("\\"), Parser.FixType(() => _symbolParsers["almost_all_escape_chars"]))))},
			{"all_whitespace", Parser.Symbol("all_whitespace", Parser.Any(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.FixType(() => _symbolParsers["all_whitespace"]))))},
			{"ignore_all_whitespace", Parser.Symbol("ignore_all_whitespace", Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["all_whitespace"])))},
			{"anychar", Parser.Symbol("anychar", Parser.Literal("*"))},
			{"range", Parser.Symbol("range", Parser.InOrder(Parser.Literal("["), Parser.Any(Parser.Literal("\\]"), Parser.Literal("\\-"), Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal("-"), Parser.Literal("]"))), Parser.AnyChar())), Parser.Literal("-"), Parser.Any(Parser.Literal("\\]"), Parser.Literal("\\-"), Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal("-"), Parser.Literal("]"))), Parser.AnyChar())), Parser.Literal("]")))},
			{"allchars_not_quote", Parser.Symbol("allchars_not_quote", Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal("'"), Parser.InOrder(Parser.FixType(() => _symbolParsers["escaped"]), Parser.FixType(() => _symbolParsers["almost_all_escape_chars"]), Parser.Literal("\\'")))), Parser.Any(Parser.InOrder(Parser.Negate(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"]))), Parser.FixType(() => _symbolParsers["all_escape_chars"]), Parser.AnyChar()), Parser.InOrder(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"])), Parser.FixType(() => _symbolParsers["all_escape_chars"])), Parser.AnyChar()), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["allchars_not_quote"]))))},
			{"literal", Parser.Symbol("literal", Parser.InOrder(Parser.Literal("'"), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["allchars_not_quote"])), Parser.Literal("'")))},
			{"allchars_not_gt", Parser.Symbol("allchars_not_gt", Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal(">"), Parser.InOrder(Parser.FixType(() => _symbolParsers["escaped"]), Parser.FixType(() => _symbolParsers["almost_all_escape_chars"]), Parser.Literal("\\>")))), Parser.Any(Parser.InOrder(Parser.Negate(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"]))), Parser.FixType(() => _symbolParsers["all_escape_chars"]), Parser.AnyChar()), Parser.InOrder(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"])), Parser.FixType(() => _symbolParsers["all_escape_chars"])), Parser.AnyChar()), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["allchars_not_gt"]))))},
			{"symbol", Parser.Symbol("symbol", Parser.InOrder(Parser.Literal("<"), Parser.FixType(() => _symbolParsers["allchars_not_gt"]), Parser.Literal(">")))},
			{"inorder_ele", Parser.Symbol("inorder_ele", Parser.InOrder(Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Any(Parser.InOrder(Parser.FixType(() => _symbolParsers["symbol"]), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Negate(Parser.Literal("="))), Parser.FixType(() => _symbolParsers["literal"]), Parser.FixType(() => _symbolParsers["group"]), Parser.FixType(() => _symbolParsers["negation"]), Parser.FixType(() => _symbolParsers["anychar"]), Parser.FixType(() => _symbolParsers["range"]))))},
			{"inorder", Parser.Symbol("inorder", Parser.Any(Parser.FixType(() => _symbolParsers["inorder_ele"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["inorder_ele"]), Parser.FixType(() => _symbolParsers["inorder"]))))},
			{"all_inorder", Parser.Symbol("all_inorder", Parser.FixType(() => _symbolParsers["inorder"]))},
			{"any", Parser.Symbol("any", Parser.Any(Parser.FixType(() => _symbolParsers["all_inorder"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["all_inorder"]), Parser.Literal("|"), Parser.FixType(() => _symbolParsers["any"]))))},
			{"all_any", Parser.Symbol("all_any", Parser.FixType(() => _symbolParsers["any"]))},
			{"group", Parser.Symbol("group", Parser.InOrder(Parser.Literal("("), Parser.FixType(() => _symbolParsers["all_any"]), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal(")")))},
			{"negation", Parser.Symbol("negation", Parser.InOrder(Parser.Literal("-"), Parser.Any(Parser.FixType(() => _symbolParsers["group"]), Parser.FixType(() => _symbolParsers["symbol"]), Parser.FixType(() => _symbolParsers["literal"]), Parser.FixType(() => _symbolParsers["negation"]), Parser.FixType(() => _symbolParsers["range"]), Parser.FixType(() => _symbolParsers["anychar"]))))},
			{"assignment", Parser.Symbol("assignment", Parser.InOrder(Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("<"), Parser.FixType(() => _symbolParsers["allchars_not_gt"]), Parser.Literal(">"), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("="), Parser.FixType(() => _symbolParsers["all_any"]), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"])))},
			{"fallback_point", Parser.Symbol("fallback_point", Parser.InOrder(Parser.Literal("f"), Parser.FixType(() => _symbolParsers["assignment"])))},
			{"all_assignments", Parser.FallbackPoint(Parser.Symbol("all_assignments", Parser.InOrder(Parser.Any(Parser.FixType(() => _symbolParsers["assignment"]), Parser.FixType(() => _symbolParsers["fallback_point"])), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["all_assignments"])))))},
			{"all_all_assignments", Parser.Symbol("all_all_assignments", Parser.FixType(() => _symbolParsers["all_assignments"]))},
			{"EntryPoint", Parser.Symbol("EntryPoint", Parser.FixType(() => _symbolParsers["all_all_assignments"]))}
        };

        public static Tuple<List<object>, List<Error>> Execute(string code) {
            var parserResult = new Result();
            var errors = new List<Error>();
            _symbolParsers["EntryPoint"](code, 0, parserResult, errors);
            return Tuple.Create(Interpreter.RecursiveEval(parserResult, _symbolHandlers.ToDictionary(x => x.SymbolName)), errors);
        }
    }
}