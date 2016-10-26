//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicInterpreter {
    public static partial class LoadLanguageInterpreter {
        static IReadOnlyDictionary<string, Parse> _symbolParsers = new Dictionary<string, Parse>() {
            {"newline", Parser.Symbol("newline", Parser.Any(Parser.Literal("\r\n"), Parser.Literal("\n")))},
			{"whitespace", Parser.Symbol("whitespace", Parser.Any(Parser.Literal(" "), Parser.Literal("\t"), Parser.FixType(() => _symbolParsers["newline"])))},
			{"escaped", Parser.Symbol("escaped", Parser.Any(Parser.InOrder(Parser.Literal("\\"), Parser.Negate(Parser.Literal("\\"))), Parser.InOrder(Parser.Literal("\\\\"), Parser.FixType(() => _symbolParsers["escaped"]))))},
			{"all_escape_chars", Parser.Symbol("all_escape_chars", Parser.Any(Parser.Literal("\\"), Parser.InOrder(Parser.Literal("\\"), Parser.FixType(() => _symbolParsers["all_escape_chars"]))))},
			{"almost_all_escape_chars", Parser.Symbol("almost_all_escape_chars", Parser.Any(Parser.InOrder(Parser.Literal("\\"), Parser.Negate(Parser.Negate(Parser.Literal("\\")))), Parser.InOrder(Parser.Literal("\\"), Parser.FixType(() => _symbolParsers["almost_all_escape_chars"]))))},
			{"all_whitespace", Parser.Symbol("all_whitespace", Parser.Any(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.FixType(() => _symbolParsers["all_whitespace"]))))},
			{"ignore_all_whitespace", Parser.Symbol("ignore_all_whitespace", Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["all_whitespace"])))},
			{"anychar", Parser.Symbol("anychar", Parser.Literal("."))},
			{"range", Parser.Symbol("range", Parser.InOrder(Parser.Literal("["), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Any(Parser.Literal("\\]"), Parser.Literal("\\:"), Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal(":"), Parser.Literal("]"))), Parser.AnyChar())), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal(":"), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Any(Parser.Literal("\\]"), Parser.Literal("\\:"), Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal(":"), Parser.Literal("]"))), Parser.AnyChar())), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("]")))},
			{"comment_text", Parser.Symbol("comment_text", Parser.InOrder(Parser.InOrder(Parser.Negate(Parser.FixType(() => _symbolParsers["newline"])), Parser.AnyChar()), Parser.Any(Parser.FixType(() => _symbolParsers["comment_text"]), Parser.Literal(""))))},
			{"comment", Parser.Symbol("comment", Parser.InOrder(Parser.Literal("//"), Parser.FixType(() => _symbolParsers["comment_text"])))},
			{"allchars_not_quote", Parser.Symbol("allchars_not_quote", Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal("'"), Parser.InOrder(Parser.FixType(() => _symbolParsers["escaped"]), Parser.FixType(() => _symbolParsers["almost_all_escape_chars"]), Parser.Literal("\\'")))), Parser.Any(Parser.InOrder(Parser.Negate(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"]))), Parser.FixType(() => _symbolParsers["all_escape_chars"]), Parser.AnyChar()), Parser.InOrder(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"])), Parser.FixType(() => _symbolParsers["all_escape_chars"])), Parser.AnyChar()), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["allchars_not_quote"]))))},
			{"literal", Parser.Symbol("literal", Parser.InOrder(Parser.Literal("'"), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["allchars_not_quote"])), Parser.Literal("'")))},
			{"allchars_not_gt", Parser.Symbol("allchars_not_gt", Parser.InOrder(Parser.Negate(Parser.Any(Parser.Literal(">"), Parser.InOrder(Parser.FixType(() => _symbolParsers["escaped"]), Parser.FixType(() => _symbolParsers["almost_all_escape_chars"]), Parser.Literal("\\>")))), Parser.Any(Parser.InOrder(Parser.Negate(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"]))), Parser.FixType(() => _symbolParsers["all_escape_chars"]), Parser.AnyChar()), Parser.InOrder(Parser.Negate(Parser.FixType(() => _symbolParsers["escaped"])), Parser.FixType(() => _symbolParsers["all_escape_chars"])), Parser.AnyChar()), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["allchars_not_gt"]))))},
			{"symbol", Parser.Symbol("symbol", Parser.InOrder(Parser.Literal("<"), Parser.FixType(() => _symbolParsers["allchars_not_gt"]), Parser.Literal(">")))},
			{"inorder_ele", Parser.Symbol("inorder_ele", Parser.InOrder(Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Any(Parser.InOrder(Parser.FixType(() => _symbolParsers["symbol"]), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Negate(Parser.Literal("="))), Parser.FixType(() => _symbolParsers["literal"]), Parser.FixType(() => _symbolParsers["group"]), Parser.FixType(() => _symbolParsers["negation"]), Parser.FixType(() => _symbolParsers["anychar"]), Parser.FixType(() => _symbolParsers["range"]), Parser.FixType(() => _symbolParsers["repeat"]))))},
			{"inorder", Parser.Symbol("inorder", Parser.Any(Parser.FixType(() => _symbolParsers["inorder_ele"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["inorder_ele"]), Parser.FixType(() => _symbolParsers["inorder"]))))},
			{"all_inorder", Parser.Symbol("all_inorder", Parser.FixType(() => _symbolParsers["inorder"]))},
			{"any", Parser.Symbol("any", Parser.Any(Parser.FixType(() => _symbolParsers["all_inorder"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["all_inorder"]), Parser.Literal("|"), Parser.FixType(() => _symbolParsers["any"]))))},
			{"all_any", Parser.Symbol("all_any", Parser.FixType(() => _symbolParsers["any"]))},
			{"group", Parser.Symbol("group", Parser.InOrder(Parser.Literal("("), Parser.FixType(() => _symbolParsers["all_any"]), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal(")")))},
			{"negation", Parser.Symbol("negation", Parser.InOrder(Parser.Literal("-"), Parser.Any(Parser.FixType(() => _symbolParsers["group"]), Parser.FixType(() => _symbolParsers["symbol"]), Parser.FixType(() => _symbolParsers["literal"]), Parser.FixType(() => _symbolParsers["negation"]), Parser.FixType(() => _symbolParsers["range"]), Parser.FixType(() => _symbolParsers["anychar"]), Parser.FixType(() => _symbolParsers["repeat"]))))},
			{"integer", Parser.Symbol("integer", Parser.Any(Parser.Range('0', '9'), Parser.InOrder(Parser.Range('0', '9'), Parser.FixType(() => _symbolParsers["integer"]))))},
			{"repeat", Parser.Symbol("repeat", Parser.InOrder(Parser.Any(Parser.FixType(() => _symbolParsers["group"]), Parser.FixType(() => _symbolParsers["symbol"]), Parser.FixType(() => _symbolParsers["literal"]), Parser.FixType(() => _symbolParsers["range"]), Parser.FixType(() => _symbolParsers["anychar"])), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("{"), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Any(Parser.FixType(() => _symbolParsers["integer"]), Parser.Literal("")), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal(":"), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Any(Parser.FixType(() => _symbolParsers["integer"]), Parser.Literal("")), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("}")))},
			{"assignment", Parser.Symbol("assignment", Parser.InOrder(Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("<"), Parser.FixType(() => _symbolParsers["allchars_not_gt"]), Parser.Literal(">"), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"]), Parser.Literal("="), Parser.FixType(() => _symbolParsers["all_any"]), Parser.FixType(() => _symbolParsers["ignore_all_whitespace"])))},
			{"fallback_point", Parser.Symbol("fallback_point", Parser.InOrder(Parser.Literal("f"), Parser.FixType(() => _symbolParsers["assignment"])))},
			{"all_assignments_and_comments", Parser.FallbackPoint(Parser.Symbol("all_assignments_and_comments", Parser.InOrder(Parser.Any(Parser.FixType(() => _symbolParsers["assignment"]), Parser.FixType(() => _symbolParsers["fallback_point"]), Parser.FixType(() => _symbolParsers["comment"])), Parser.Any(Parser.Literal(""), Parser.FixType(() => _symbolParsers["all_assignments_and_comments"])))))},
			{"all_all_assignments_and_comments", Parser.Symbol("all_all_assignments_and_comments", Parser.FixType(() => _symbolParsers["all_assignments_and_comments"]))},
			{"EntryPoint", Parser.Symbol("EntryPoint", Parser.FixType(() => _symbolParsers["all_all_assignments_and_comments"]))}
        };

        public static Tuple<List<object>, List<Error>> Execute(string code) {
            var parserResult = new Result();
            var parserErrors = new List<Error>();
            _symbolParsers["EntryPoint"](code, 0, parserResult, parserErrors);
            
            return Interpreter.RecursiveEval(
                parserResult, 
                _symbolHandlers.ToDictionary(x => x.SymbolName)
            ).Match<Tuple<List<object>, List<Error>>>(
                result => Tuple.Create(result, parserErrors), 
                err => {
                    parserErrors.Insert(0, err);
                    return Tuple.Create(new List<object>(), parserErrors);
                }
            );
        }
    }
}