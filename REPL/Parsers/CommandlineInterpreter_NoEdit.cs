//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicInterpreter {
    public static partial class CommandlineInterpreter {
        static IReadOnlyDictionary<string, Parse> _symbolParsers = new Dictionary<string, Parse>() {
            {"uppercase", Parser.Symbol("uppercase", Parser.Any(Parser.Literal("A"), Parser.Literal("B"), Parser.Literal("C"), Parser.Literal("D"), Parser.Literal("E"), Parser.Literal("F"), Parser.Literal("G"), Parser.Literal("H"), Parser.Literal("I"), Parser.Literal("J"), Parser.Literal("K"), Parser.Literal("L"), Parser.Literal("M"), Parser.Literal("N"), Parser.Literal("O"), Parser.Literal("P"), Parser.Literal("Q"), Parser.Literal("R"), Parser.Literal("S"), Parser.Literal("T"), Parser.Literal("U"), Parser.Literal("V"), Parser.Literal("W"), Parser.Literal("X"), Parser.Literal("Y"), Parser.Literal("Z")))},
			{"lowercase", Parser.Symbol("lowercase", Parser.Any(Parser.Literal("a"), Parser.Literal("b"), Parser.Literal("c"), Parser.Literal("d"), Parser.Literal("e"), Parser.Literal("f"), Parser.Literal("g"), Parser.Literal("h"), Parser.Literal("i"), Parser.Literal("j"), Parser.Literal("k"), Parser.Literal("l"), Parser.Literal("m"), Parser.Literal("n"), Parser.Literal("o"), Parser.Literal("p"), Parser.Literal("q"), Parser.Literal("r"), Parser.Literal("s"), Parser.Literal("t"), Parser.Literal("u"), Parser.Literal("v"), Parser.Literal("w"), Parser.Literal("x"), Parser.Literal("y"), Parser.Literal("z")))},
			{"digits", Parser.Symbol("digits", Parser.Any(Parser.Literal("0"), Parser.Literal("1"), Parser.Literal("2"), Parser.Literal("3"), Parser.Literal("4"), Parser.Literal("5"), Parser.Literal("6"), Parser.Literal("7"), Parser.Literal("8"), Parser.Literal("9")))},
			{"whitespace", Parser.Symbol("whitespace", Parser.Any(Parser.Literal(" "), Parser.Literal("\t"), Parser.Literal("\r\n"), Parser.Literal("\n")))},
			{"specialchars", Parser.Symbol("specialchars", Parser.Any(Parser.Literal("!"), Parser.Literal("#"), Parser.Literal("$"), Parser.Literal("%"), Parser.Literal("&"), Parser.Literal("'"), Parser.Literal("("), Parser.Literal(")"), Parser.Literal("*"), Parser.Literal("+"), Parser.Literal(","), Parser.Literal("."), Parser.Literal("/"), Parser.Literal(":"), Parser.Literal(";"), Parser.Literal("<"), Parser.Literal(">"), Parser.Literal("?"), Parser.Literal("@"), Parser.Literal("["), Parser.Literal("]"), Parser.Literal("^"), Parser.Literal("_"), Parser.Literal("`"), Parser.Literal("{"), Parser.Literal("|"), Parser.Literal("}"), Parser.Literal("~"), Parser.Literal("\\"), Parser.Literal("\""), Parser.Literal("="), Parser.Literal("-")))},
			{"specialchars_no_quotes_equals_or_dash", Parser.Symbol("specialchars_no_quotes_equals_or_dash", Parser.InOrder(Parser.Negate(Parser.Literal("\"")), Parser.Negate(Parser.Literal("=")), Parser.Negate(Parser.Literal("-")), Parser.FixType(() => _symbolParsers["specialchars"])))},
			{"specialchars_no_quotes_or_equals", Parser.Symbol("specialchars_no_quotes_or_equals", Parser.Any(Parser.FixType(() => _symbolParsers["specialchars_no_quotes_equals_or_dash"]), Parser.Literal("-")))},
			{"specialchars_noquotes", Parser.Symbol("specialchars_noquotes", Parser.Any(Parser.FixType(() => _symbolParsers["specialchars_no_quotes_or_equals"]), Parser.Literal("=")))},
			{"chars_no_quotes_equals_or_whitespace", Parser.Symbol("chars_no_quotes_equals_or_whitespace", Parser.Any(Parser.FixType(() => _symbolParsers["uppercase"]), Parser.FixType(() => _symbolParsers["lowercase"]), Parser.FixType(() => _symbolParsers["digits"]), Parser.FixType(() => _symbolParsers["specialchars_no_quotes_or_equals"])))},
			{"chars_no_quotes", Parser.Symbol("chars_no_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["chars_no_quotes_equals_or_whitespace"]), Parser.Literal("="), Parser.FixType(() => _symbolParsers["whitespace"])))},
			{"chars_no_dash", Parser.Symbol("chars_no_dash", Parser.Any(Parser.FixType(() => _symbolParsers["specialchars_no_quotes_equals_or_dash"]), Parser.Literal("="), Parser.Literal("\""), Parser.FixType(() => _symbolParsers["uppercase"]), Parser.FixType(() => _symbolParsers["lowercase"]), Parser.FixType(() => _symbolParsers["digits"]), Parser.FixType(() => _symbolParsers["whitespace"])))},
			{"allchars_no_dash", Parser.Symbol("allchars_no_dash", Parser.Any(Parser.FixType(() => _symbolParsers["chars_no_dash"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["chars_no_dash"]), Parser.FixType(() => _symbolParsers["allchars_no_dash"]))))},
			{"chars_no_quotes_or_whitespace", Parser.Symbol("chars_no_quotes_or_whitespace", Parser.Any(Parser.FixType(() => _symbolParsers["lowercase"]), Parser.FixType(() => _symbolParsers["digits"]), Parser.FixType(() => _symbolParsers["uppercase"]), Parser.FixType(() => _symbolParsers["specialchars_noquotes"])))},
			{"allchars_no_quotes", Parser.Symbol("allchars_no_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["chars_no_quotes"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["chars_no_quotes"]), Parser.FixType(() => _symbolParsers["allchars_no_quotes"]))))},
			{"allchars_no_quotes_or_equals_or_whitespace", Parser.Symbol("allchars_no_quotes_or_equals_or_whitespace", Parser.Any(Parser.FixType(() => _symbolParsers["chars_no_quotes_equals_or_whitespace"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["chars_no_quotes_equals_or_whitespace"]), Parser.FixType(() => _symbolParsers["allchars_no_quotes_or_equals_or_whitespace"]))))},
			{"chars_no_quotes", Parser.Symbol("chars_no_quotes", Parser.Any(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.FixType(() => _symbolParsers["chars_no_quotes_or_whitespace"])))},
			{"allwhitespace", Parser.Symbol("allwhitespace", Parser.Any(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["whitespace"]), Parser.FixType(() => _symbolParsers["allwhitespace"]))))},
			{"argname_value_separator", Parser.Symbol("argname_value_separator", Parser.Any(Parser.FixType(() => _symbolParsers["allwhitespace"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["allwhitespace"]), Parser.Literal("="), Parser.FixType(() => _symbolParsers["allwhitespace"]))))},
			{"argvalue", Parser.Symbol("argvalue", Parser.Any(Parser.InOrder(Parser.Literal("\""), Parser.FixType(() => _symbolParsers["allchars_no_quotes"]), Parser.Literal("\"")), Parser.FixType(() => _symbolParsers["allchars_no_quotes_or_equals_or_whitespace"])))},
			{"argvalues", Parser.Symbol("argvalues", Parser.Any(Parser.FixType(() => _symbolParsers["argvalue"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["argvalue"]), Parser.FixType(() => _symbolParsers["allwhitespace"]), Parser.FixType(() => _symbolParsers["argvalues"]))))},
			{"argname", Parser.Symbol("argname", Parser.FixType(() => _symbolParsers["allchars_no_quotes_or_equals_or_whitespace"]))},
			{"arg", Parser.Symbol("arg", Parser.Any(Parser.InOrder(Parser.Literal("-"), Parser.FixType(() => _symbolParsers["argname"])), Parser.InOrder(Parser.Literal("-"), Parser.FixType(() => _symbolParsers["argname"]), Parser.FixType(() => _symbolParsers["argname_value_separator"]), Parser.FixType(() => _symbolParsers["argvalues"]))))},
			{"args", Parser.Symbol("args", Parser.Any(Parser.FixType(() => _symbolParsers["arg"]), Parser.InOrder(Parser.FixType(() => _symbolParsers["arg"]), Parser.FixType(() => _symbolParsers["args"]))))},
			{"EntryPoint", Parser.Symbol("EntryPoint", Parser.InOrder(Parser.FixType(() => _symbolParsers["allchars_no_dash"]), Parser.FixType(() => _symbolParsers["args"])))}
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