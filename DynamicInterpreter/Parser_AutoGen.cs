using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DynamicInterpreter {
    public static partial class Parser {
        public enum State { Success, Failure }
        public class Result : List<Union<string, Tuple<string, Result>>> { }
        public delegate Tuple<State, string> Parse(string data, Result acc);

        public static IReadOnlyDictionary<string, Union<Parse, Func<Parse>>> SymbolParsers = new Dictionary<string, Union<Parse, Func<Parse>>>(){
            {"uppercase", Symbol("uppercase", Any(Literal("A"), Literal("B"), Literal("C"), Literal("D"), Literal("E"), Literal("F"), Literal("G"), Literal("H"), Literal("I"), Literal("J"), Literal("K"), Literal("L"), Literal("M"), Literal("N"), Literal("O"), Literal("P"), Literal("Q"), Literal("R"), Literal("S"), Literal("T"), Literal("U"), Literal("V"), Literal("W"), Literal("X"), Literal("Y"), Literal("Z")))},
			{"lowercase", Symbol("lowercase", Any(Literal("a"), Literal("b"), Literal("c"), Literal("d"), Literal("e"), Literal("f"), Literal("g"), Literal("h"), Literal("i"), Literal("j"), Literal("k"), Literal("l"), Literal("m"), Literal("n"), Literal("o"), Literal("p"), Literal("q"), Literal("r"), Literal("s"), Literal("t"), Literal("u"), Literal("v"), Literal("w"), Literal("x"), Literal("y"), Literal("z")))},
			{"digits", Symbol("digits", Any(Literal("0"), Literal("1"), Literal("2"), Literal("3"), Literal("4"), Literal("5"), Literal("6"), Literal("7"), Literal("8"), Literal("9")))},
			{"whitespace", Symbol("whitespace", Any(Literal(" "), Literal("\t"), Literal("\r\n"), Literal("\n")))},
			{"specialchars", Symbol("specialchars", Any(Literal("!"), Literal("#"), Literal("$"), Literal("%"), Literal("&"), Literal("'"), Literal("("), Literal(")"), Literal("*"), Literal("+"), Literal(","), Literal("."), Literal("/"), Literal(":"), Literal(";"), Literal("<"), Literal(">"), Literal("?"), Literal("@"), Literal("["), Literal("]"), Literal("^"), Literal("_"), Literal("`"), Literal("{"), Literal("|"), Literal("}"), Literal("~"), Literal("\\"), Literal("\""), Literal("="), Literal("-")))},
			{"escaped", Symbol("escaped", Any(InOrder(Literal("\\"), Negate(Literal("\\"))), InOrder(Literal("\\\\"), FixType(() => Eval(SymbolParsers["escaped"])))))},
			{"all_escape_chars", Symbol("all_escape_chars", Any(Literal("\\"), InOrder(Literal("\\"), FixType(() => Eval(SymbolParsers["all_escape_chars"])))))},
			{"almost_all_escape_chars", Symbol("almost_all_escape_chars", Any(InOrder(Literal("\\"), Negate(Negate(Literal("\\")))), InOrder(Literal("\\"), FixType(() => Eval(SymbolParsers["almost_all_escape_chars"])))))},
			{"anychar", Symbol("anychar", Any(FixType(() => Eval(SymbolParsers["uppercase"])), FixType(() => Eval(SymbolParsers["lowercase"])), FixType(() => Eval(SymbolParsers["digits"])), FixType(() => Eval(SymbolParsers["whitespace"])), FixType(() => Eval(SymbolParsers["specialchars"]))))},
			{"all_whitespace", Symbol("all_whitespace", Any(FixType(() => Eval(SymbolParsers["whitespace"])), InOrder(FixType(() => Eval(SymbolParsers["whitespace"])), FixType(() => Eval(SymbolParsers["all_whitespace"])))))},
			{"ignore_all_whitespace", Symbol("ignore_all_whitespace", Any(Literal(""), FixType(() => Eval(SymbolParsers["all_whitespace"]))))},
			{"allchars_not_quote", Symbol("allchars_not_quote", InOrder(Negate(Any(Literal("'"), InOrder(FixType(() => Eval(SymbolParsers["escaped"])), FixType(() => Eval(SymbolParsers["almost_all_escape_chars"])), Literal("\\'")))), Any(InOrder(Negate(Negate(FixType(() => Eval(SymbolParsers["escaped"])))), FixType(() => Eval(SymbolParsers["all_escape_chars"])), FixType(() => Eval(SymbolParsers["anychar"]))), InOrder(Negate(FixType(() => Eval(SymbolParsers["escaped"]))), FixType(() => Eval(SymbolParsers["all_escape_chars"]))), FixType(() => Eval(SymbolParsers["anychar"]))), Any(Literal(""), FixType(() => Eval(SymbolParsers["allchars_not_quote"])))))},
			{"literal", Symbol("literal", InOrder(Literal("'"), Any(Literal(""), FixType(() => Eval(SymbolParsers["allchars_not_quote"]))), Literal("'")))},
			{"allchars_not_gt", Symbol("allchars_not_gt", InOrder(Negate(Any(Literal(">"), InOrder(FixType(() => Eval(SymbolParsers["escaped"])), FixType(() => Eval(SymbolParsers["almost_all_escape_chars"])), Literal("\\>")))), Any(InOrder(Negate(Negate(FixType(() => Eval(SymbolParsers["escaped"])))), FixType(() => Eval(SymbolParsers["all_escape_chars"])), FixType(() => Eval(SymbolParsers["anychar"]))), InOrder(Negate(FixType(() => Eval(SymbolParsers["escaped"]))), FixType(() => Eval(SymbolParsers["all_escape_chars"]))), FixType(() => Eval(SymbolParsers["anychar"]))), Any(Literal(""), FixType(() => Eval(SymbolParsers["allchars_not_gt"])))))},
			{"symbol", Symbol("symbol", InOrder(Literal("<"), FixType(() => Eval(SymbolParsers["allchars_not_gt"])), Literal(">")))},
			{"inorder_ele", Symbol("inorder_ele", InOrder(FixType(() => Eval(SymbolParsers["ignore_all_whitespace"])), Any(InOrder(FixType(() => Eval(SymbolParsers["symbol"])), FixType(() => Eval(SymbolParsers["ignore_all_whitespace"])), Negate(Literal("="))), FixType(() => Eval(SymbolParsers["literal"])), FixType(() => Eval(SymbolParsers["group"])), FixType(() => Eval(SymbolParsers["negation"])))))},
			{"inorder", Symbol("inorder", Any(FixType(() => Eval(SymbolParsers["inorder_ele"])), InOrder(FixType(() => Eval(SymbolParsers["inorder_ele"])), FixType(() => Eval(SymbolParsers["inorder"])))))},
			{"all_inorder", Symbol("all_inorder", FixType(() => Eval(SymbolParsers["inorder"])))},
			{"any", Symbol("any", Any(FixType(() => Eval(SymbolParsers["all_inorder"])), InOrder(FixType(() => Eval(SymbolParsers["all_inorder"])), Literal("|"), FixType(() => Eval(SymbolParsers["any"])))))},
			{"all_any", Symbol("all_any", FixType(() => Eval(SymbolParsers["any"])))},
			{"group", Symbol("group", InOrder(Literal("("), FixType(() => Eval(SymbolParsers["all_any"])), FixType(() => Eval(SymbolParsers["ignore_all_whitespace"])), Literal(")")))},
			{"negation", Symbol("negation", InOrder(Literal("-"), Any(FixType(() => Eval(SymbolParsers["group"])), FixType(() => Eval(SymbolParsers["symbol"])), FixType(() => Eval(SymbolParsers["literal"])), FixType(() => Eval(SymbolParsers["negation"])))))},
			{"assignment", Symbol("assignment", InOrder(FixType(() => Eval(SymbolParsers["ignore_all_whitespace"])), Literal("<"), FixType(() => Eval(SymbolParsers["allchars_not_gt"])), Literal(">"), FixType(() => Eval(SymbolParsers["ignore_all_whitespace"])), Literal("="), FixType(() => Eval(SymbolParsers["all_any"])), FixType(() => Eval(SymbolParsers["ignore_all_whitespace"]))))},
			{"all_assignments", Symbol("all_assignments", Any(FixType(() => Eval(SymbolParsers["assignment"])), InOrder(FixType(() => Eval(SymbolParsers["assignment"])), FixType(() => Eval(SymbolParsers["all_assignments"])))))},
			{"EntryPoint", Symbol("EntryPoint", FixType(() => Eval(SymbolParsers["all_assignments"])))}
        };

        private static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {
            return (data, acc) => {
                var newAcc = new Result();
                var result = Eval(parser)(data, newAcc);
                if (result.Item1 == State.Success) acc.Add(Tuple.Create(symbolName, newAcc));
                return result;
            };
        }

        private static Parse Literal(string value) {
            return (data, acc) => {
                if(data.StartsWith(value)) {
                    acc.Add(value);
                    return Tuple.Create(State.Success, data.Remove(0, value.Length));
                }
                return Tuple.Create(State.Failure, data);
            };
        }

        private static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {
            return (data, acc) => {
                var newAcc = new Result();
                var result = Eval(parserToNegate)(data, newAcc);
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data);
                return Tuple.Create(State.Success, data);
            };
        }

        private static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, acc) => {
                var newAcc = new Result();
                var result = data;
                foreach (var parser in parsers) {
                    var partialResult = Eval(parser)(result, newAcc);
                    if (partialResult.Item1 == State.Failure) return Tuple.Create(State.Failure, data);
                    result = partialResult.Item2;
                }
                acc.AddRange(newAcc);
                return Tuple.Create(State.Success, result);
            };
        }

        private static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, acc) => {
                var result = parsers.Select(
                    x => {
                        var newAcc = new Result();
                        return new { result = Eval(x)(data, newAcc), acc = newAcc };
                    }
                ).Where(x => x.result.Item1 == State.Success).ToArray();

                if(result.Length == 0) return Tuple.Create(State.Failure, data);
                var longestMatch = result.MinBy(x => x.result.Item2.Length);
                acc.AddRange(longestMatch.acc);
                return longestMatch.result;
            };
        }

        private static Parse Eval(Union<Parse, Func<Parse>> val) {
            return val.Match<Parse>(x => x, x => x());
        }

        private static Func<Parse> FixType(Func<Parse> parse) {
            return parse;
        }
    }
}