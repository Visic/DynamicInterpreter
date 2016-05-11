using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DynamicInterpreter {
    public static partial class Parser {
        public class Error {
            public Error(string message, int characterIndex) {
                Message = message;
                CharacterIndex = characterIndex;
            }

            public Error(string message, List<Error> subErrors, int characterIndex) : this (message, characterIndex) {
                SubErrors = subErrors;
            }

            public string Message { get; }
            public int CharacterIndex { get; }
            public List<Error> SubErrors { get; } = new List<Error>();
        }

        public enum State { Success, Failure }
        public class Result : List<Union<string, Tuple<string, Result>>> { }
        public delegate Tuple<State, string> Parse(string data, Result acc, List<Error> errors);

        public static IReadOnlyDictionary<string, Parse> SymbolParsers = new Dictionary<string, Parse>() {
            {"uppercase", Symbol("uppercase", Any(Literal("A"), Literal("B"), Literal("C"), Literal("D"), Literal("E"), Literal("F"), Literal("G"), Literal("H"), Literal("I"), Literal("J"), Literal("K"), Literal("L"), Literal("M"), Literal("N"), Literal("O"), Literal("P"), Literal("Q"), Literal("R"), Literal("S"), Literal("T"), Literal("U"), Literal("V"), Literal("W"), Literal("X"), Literal("Y"), Literal("Z")))},
            {"lowercase", Symbol("lowercase", Any(Literal("a"), Literal("b"), Literal("c"), Literal("d"), Literal("e"), Literal("f"), Literal("g"), Literal("h"), Literal("i"), Literal("j"), Literal("k"), Literal("l"), Literal("m"), Literal("n"), Literal("o"), Literal("p"), Literal("q"), Literal("r"), Literal("s"), Literal("t"), Literal("u"), Literal("v"), Literal("w"), Literal("x"), Literal("y"), Literal("z")))},
            {"digits", Symbol("digits", Any(Literal("0"), Literal("1"), Literal("2"), Literal("3"), Literal("4"), Literal("5"), Literal("6"), Literal("7"), Literal("8"), Literal("9")))},
            {"whitespace", Symbol("whitespace", Any(Literal(" "), Literal("\t"), Literal("\r\n"), Literal("\n")))},
            {"specialchars", Symbol("specialchars", Any(Literal("!"), Literal("#"), Literal("$"), Literal("%"), Literal("&"), Literal("'"), Literal("("), Literal(")"), Literal("*"), Literal("+"), Literal(","), Literal("."), Literal("/"), Literal(":"), Literal(";"), Literal("<"), Literal(">"), Literal("?"), Literal("@"), Literal("["), Literal("]"), Literal("^"), Literal("_"), Literal("`"), Literal("{"), Literal("|"), Literal("}"), Literal("~"), Literal("\\"), Literal("\""), Literal("="), Literal("-")))},
            {"escaped", Symbol("escaped", Any(InOrder(Literal("\\"), Negate(Literal("\\"))), InOrder(Literal("\\\\"), FixType(() => SymbolParsers["escaped"]))))},
            {"all_escape_chars", Symbol("all_escape_chars", Any(Literal("\\"), InOrder(Literal("\\"), FixType(() => SymbolParsers["all_escape_chars"]))))},
            {"almost_all_escape_chars", Symbol("almost_all_escape_chars", Any(InOrder(Literal("\\"), Negate(Negate(Literal("\\")))), InOrder(Literal("\\"), FixType(() => SymbolParsers["almost_all_escape_chars"]))))},
            {"anychar", Symbol("anychar", Any(FixType(() => SymbolParsers["uppercase"]), FixType(() => SymbolParsers["lowercase"]), FixType(() => SymbolParsers["digits"]), FixType(() => SymbolParsers["whitespace"]), FixType(() => SymbolParsers["specialchars"])))},
            {"all_whitespace", Symbol("all_whitespace", Any(FixType(() => SymbolParsers["whitespace"]), InOrder(FixType(() => SymbolParsers["whitespace"]), FixType(() => SymbolParsers["all_whitespace"]))))},
            {"ignore_all_whitespace", Symbol("ignore_all_whitespace", Any(Literal(""), FixType(() => SymbolParsers["all_whitespace"])))},
            {"allchars_not_quote", Symbol("allchars_not_quote", InOrder(Negate(Any(Literal("'"), InOrder(FixType(() => SymbolParsers["escaped"]), FixType(() => SymbolParsers["almost_all_escape_chars"]), Literal("\\'")))), Any(InOrder(Negate(Negate(FixType(() => SymbolParsers["escaped"]))), FixType(() => SymbolParsers["all_escape_chars"]), FixType(() => SymbolParsers["anychar"])), InOrder(Negate(FixType(() => SymbolParsers["escaped"])), FixType(() => SymbolParsers["all_escape_chars"])), FixType(() => SymbolParsers["anychar"])), Any(Literal(""), FixType(() => SymbolParsers["allchars_not_quote"]))))},
            {"literal", Symbol("literal", InOrder(Literal("'"), Any(Literal(""), FixType(() => SymbolParsers["allchars_not_quote"])), Literal("'")))},
            {"allchars_not_gt", Symbol("allchars_not_gt", InOrder(Negate(Any(Literal(">"), InOrder(FixType(() => SymbolParsers["escaped"]), FixType(() => SymbolParsers["almost_all_escape_chars"]), Literal("\\>")))), Any(InOrder(Negate(Negate(FixType(() => SymbolParsers["escaped"]))), FixType(() => SymbolParsers["all_escape_chars"]), FixType(() => SymbolParsers["anychar"])), InOrder(Negate(FixType(() => SymbolParsers["escaped"])), FixType(() => SymbolParsers["all_escape_chars"])), FixType(() => SymbolParsers["anychar"])), Any(Literal(""), FixType(() => SymbolParsers["allchars_not_gt"]))))},
            {"symbol", Symbol("symbol", InOrder(Literal("<"), FixType(() => SymbolParsers["allchars_not_gt"]), Literal(">")))},
            {"inorder_ele", Symbol("inorder_ele", InOrder(FixType(() => SymbolParsers["ignore_all_whitespace"]), Any(InOrder(FixType(() => SymbolParsers["symbol"]), FixType(() => SymbolParsers["ignore_all_whitespace"]), Negate(Literal("="))), FixType(() => SymbolParsers["literal"]), FixType(() => SymbolParsers["group"]), FixType(() => SymbolParsers["negation"]))))},
            {"inorder", Symbol("inorder", Any(FixType(() => SymbolParsers["inorder_ele"]), InOrder(FixType(() => SymbolParsers["inorder_ele"]), FixType(() => SymbolParsers["inorder"]))))},
            {"all_inorder", Symbol("all_inorder", FixType(() => SymbolParsers["inorder"]))},
            {"any", Symbol("any", Any(FixType(() => SymbolParsers["all_inorder"]), InOrder(FixType(() => SymbolParsers["all_inorder"]), Literal("|"), FixType(() => SymbolParsers["any"]))))},
            {"all_any", Symbol("all_any", FixType(() => SymbolParsers["any"]))},
            {"group", Symbol("group", InOrder(Literal("("), FixType(() => SymbolParsers["all_any"]), FixType(() => SymbolParsers["ignore_all_whitespace"]), Literal(")")))},
            {"negation", Symbol("negation", InOrder(Literal("-"), Any(FixType(() => SymbolParsers["group"]), FixType(() => SymbolParsers["symbol"]), FixType(() => SymbolParsers["literal"]), FixType(() => SymbolParsers["negation"]))))},
            {"assignment", Symbol("assignment", InOrder(FixType(() => SymbolParsers["ignore_all_whitespace"]), Literal("<"), FixType(() => SymbolParsers["allchars_not_gt"]), Literal(">"), FixType(() => SymbolParsers["ignore_all_whitespace"]), Literal("="), FixType(() => SymbolParsers["all_any"]), FixType(() => SymbolParsers["ignore_all_whitespace"])))},
            {"fallback_point", Symbol("fallback_point", InOrder(Literal("f"), FixType(() => SymbolParsers["assignment"])))},
            {"all_assignments", FallbackPoint(Symbol("all_assignments", InOrder(Any(FixType(() => SymbolParsers["assignment"]), FixType(() => SymbolParsers["fallback_point"])), Any(Literal(""), FixType(() => SymbolParsers["all_assignments"])))))},
            {"all_all_assignments", Symbol("all_all_assignments", FixType(() => SymbolParsers["all_assignments"]))},
            {"EntryPoint", Symbol("EntryPoint", FixType(() => SymbolParsers["all_all_assignments"]))}
        };

        private static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {
            return (data, acc, errors) => {
                var newAcc = new Result();
                var newErrors = new List<Error>();
                var result = Eval(parser)(data, newAcc, newErrors);

                if(result.Item1 == State.Success) {
                    acc.Add(Tuple.Create(symbolName, newAcc));
                }

                if (newErrors.Count > 0) {
                    errors.Add(new Error($"Failed to parse {symbolName}", newErrors, -1));
                }

                return result;
            };
        }

        private static Parse Literal(string value) {
            return (data, acc, errors) => {
                if(data.StartsWith(value)) {
                    acc.Add(value);
                    return Tuple.Create(State.Success, data.Remove(0, value.Length));
                } else {
                    errors.Add(new Error($"Expected {value}", -1));
                    return Tuple.Create(State.Failure, data);
                }
            };
        }

        private static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {
            return (data, acc, errors) => {
                var result = Eval(parserToNegate)(data, new Result(), new List<Error>());
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data);
                return Tuple.Create(State.Success, data);
            };
        }

        private static Parse FallbackPoint(Union<Parse, Func<Parse>> parser) {
            return (data, acc, errors) => {
                var newAcc = new Result();
                var result = Eval(parser)(data, newAcc, errors);
                while(result.Item1 == State.Failure) {
                    if(data.Length == 0) break;
                    data = data.Substring(1); //while the parser failed, skip a character and try again
                    result = Eval(parser)(data, newAcc = new Result(), new List<Error>());
                }
                if(result.Item1 == State.Success) acc.AddRange(newAcc);
                return result;
            };
        }

        private static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, acc, errors) => {
                var newAcc = new Result();
                var result = data;
                foreach(var parser in parsers) {
                    var partialResult = Eval(parser)(result, newAcc, errors);
                    if(partialResult.Item1 == State.Failure) return Tuple.Create(State.Failure, data);
                    result = partialResult.Item2;
                }
                acc.AddRange(newAcc);
                return Tuple.Create(State.Success, result);
            };
        }

        private static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, acc, errors) => {
                var newErrors = new List<Error>();
                var result = parsers.Select(
                    x => {
                        var newAcc = new Result();
                        return new { result = Eval(x)(data, newAcc, newErrors), acc = newAcc };
                    }
                ).Where(x => x.result.Item1 == State.Success).ToArray();

                if(result.Length == 0) {
                    errors.Add(new Error("All of the following possibilities failed", newErrors, -1));
                    return Tuple.Create(State.Failure, data);
                }

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