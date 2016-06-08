//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public static partial class REPLCommandParser {
        static IReadOnlyDictionary<string, Parse> _symbolParsers = new Dictionary<string, Parse>() {
            {"uppercase", Parser.Symbol("uppercase", Parser.Any(Parser.Literal("A"), Parser.Literal("B"), Parser.Literal("C"), Parser.Literal("D"), Parser.Literal("E"), Parser.Literal("F"), Parser.Literal("G"), Parser.Literal("H"), Parser.Literal("I"), Parser.Literal("J"), Parser.Literal("K"), Parser.Literal("L"), Parser.Literal("M"), Parser.Literal("N"), Parser.Literal("O"), Parser.Literal("P"), Parser.Literal("Q"), Parser.Literal("R"), Parser.Literal("S"), Parser.Literal("T"), Parser.Literal("U"), Parser.Literal("V"), Parser.Literal("W"), Parser.Literal("X"), Parser.Literal("Y"), Parser.Literal("Z")))},
			{"lowercase", Parser.Symbol("lowercase", Parser.Any(Parser.Literal("a"), Parser.Literal("b"), Parser.Literal("c"), Parser.Literal("d"), Parser.Literal("e"), Parser.Literal("f"), Parser.Literal("g"), Parser.Literal("h"), Parser.Literal("i"), Parser.Literal("j"), Parser.Literal("k"), Parser.Literal("l"), Parser.Literal("m"), Parser.Literal("n"), Parser.Literal("o"), Parser.Literal("p"), Parser.Literal("q"), Parser.Literal("r"), Parser.Literal("s"), Parser.Literal("t"), Parser.Literal("u"), Parser.Literal("v"), Parser.Literal("w"), Parser.Literal("x"), Parser.Literal("y"), Parser.Literal("z")))},
			{"digit", Parser.Symbol("digit", Parser.Any(Parser.Literal("0"), Parser.Literal("1"), Parser.Literal("2"), Parser.Literal("3"), Parser.Literal("4"), Parser.Literal("5"), Parser.Literal("6"), Parser.Literal("7"), Parser.Literal("8"), Parser.Literal("9")))},
			{"whitespace", Parser.Symbol("whitespace", Parser.Any(Parser.Literal(" "), Parser.Literal("\t"), Parser.Literal("\r\n"), Parser.Literal("\n")))},
			{"specialchars_no_quotes", Parser.Symbol("specialchars_no_quotes", Parser.Any(Parser.Literal("!"), Parser.Literal("#"), Parser.Literal("$"), Parser.Literal("%"), Parser.Literal("&"), Parser.Literal("'"), Parser.Literal("("), Parser.Literal(")"), Parser.Literal("*"), Parser.Literal("+"), Parser.Literal(","), Parser.Literal("."), Parser.Literal("/"), Parser.Literal(":"), Parser.Literal(";"), Parser.Literal("<"), Parser.Literal(">"), Parser.Literal("?"), Parser.Literal("@"), Parser.Literal("["), Parser.Literal("]"), Parser.Literal("^"), Parser.Literal("_"), Parser.Literal("`"), Parser.Literal("{"), Parser.Literal("|"), Parser.Literal("}"), Parser.Literal("~"), Parser.Literal("\\"), Parser.Literal("-"), Parser.Literal("=")))},
			{"letter", Parser.Symbol("letter", Parser.Any(Parser.FixType(() => _symbolParsers["uppercase"]), Parser.FixType(() => _symbolParsers["lowercase"])))},
			{"alphanumeric", Parser.Symbol("alphanumeric", Parser.Any(Parser.FixType(() => _symbolParsers["letter"]), Parser.FixType(() => _symbolParsers["digit"])))},
			{"no_whitespace_or_quotes", Parser.Symbol("no_whitespace_or_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["specialchars_no_quotes"]), Parser.FixType(() => _symbolParsers["alphanumeric"])))},
			{"no_quotes", Parser.Symbol("no_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["no_whitespace_or_quotes"]), Parser.FixType(() => _symbolParsers["whitespace"])))},
			{"all_no_whitespace_or_quotes", Parser.Symbol("all_no_whitespace_or_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["no_whitespace_or_quotes"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["no_whitespace_or_quotes"]), Parser.FixType(() => _symbolParsers["all_no_whitespace_or_quotes"]))))},
			{"all_no_quotes", Parser.Symbol("all_no_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["no_quotes"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["no_quotes"]), Parser.FixType(() => _symbolParsers["all_no_quotes"]))))},
			{"arg", Parser.Symbol("arg", Parser.Any(Parser.FixType(() => _symbolParsers["all_no_whitespace_or_quotes"]), Parser.InOrder(Parser.Literal("\""), Parser.FixType(() => _symbolParsers["all_no_quotes"]), Parser.Literal("\""))))},
			{"args", Parser.Symbol("args", Parser.Any(Parser.FixType(() => _symbolParsers["arg"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["arg"]), Parser.FixType(() => _symbolParsers["whitespace"]), Parser.FixType(() => _symbolParsers["args"]))))},
			{"cmdname", Parser.Symbol("cmdname", Parser.Any(Parser.FixType(() => _symbolParsers["letter"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["letter"]), Parser.FixType(() => _symbolParsers["cmdname"]))))},
			{"command", Parser.FallbackPoint(Parser.Symbol("command", Parser.Any(Parser.FixType(() => _symbolParsers["cmdname"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["cmdname"]), Parser.FixType(() => _symbolParsers["whitespace"]), Parser.FixType(() => _symbolParsers["args"])))))},
			{"EntryPoint", Parser.Symbol("EntryPoint", Parser.FixType(() => _symbolParsers["command"]))}
        };

        public static Tuple<List<object>, List<Error>> Execute(string code) {
            var parserResult = new Result();
            var errors = new List<Error>();
            _symbolParsers["EntryPoint"](code, 0, parserResult, errors);
            return Tuple.Create(Parser.RecursiveEval(parserResult, _symbolHandlers.ToDictionary(x => x.SymbolName)), errors);
        }
    }
}