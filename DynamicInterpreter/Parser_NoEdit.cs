//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
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

            public override string ToString() {
                return $"Index {CharacterIndex}: {Message}";
            }
        }

        public enum State { Success, Failure }
        public class Result : List<Union<string, Tuple<string, Result>>> { }
        public delegate Tuple<State, string, int> Parse(string data, int charsHandledSoFar, Result acc, List<Error> errors);

        static Interpreter _interpreter = new Interpreter();
        static IReadOnlyDictionary<string, Parse> _symbolParsers = new Dictionary<string, Parse>() {
            {"whitespace", Symbol("whitespace", Any(Literal(" "), Literal("\t"), Literal("\r\n"), Literal("\n")))},
			{"escaped", Symbol("escaped", Any(InOrder(Literal("\\"), Negate(Literal("\\"))), InOrder(Literal("\\\\"), FixType(() => _symbolParsers["escaped"]))))},
			{"all_escape_chars", Symbol("all_escape_chars", Any(Literal("\\"), InOrder(Literal("\\"), FixType(() => _symbolParsers["all_escape_chars"]))))},
			{"almost_all_escape_chars", Symbol("almost_all_escape_chars", Any(InOrder(Literal("\\"), Negate(Negate(Literal("\\")))), InOrder(Literal("\\"), FixType(() => _symbolParsers["almost_all_escape_chars"]))))},
			{"all_whitespace", Symbol("all_whitespace", Any(FixType(() => _symbolParsers["whitespace"]), InOrder(FixType(() => _symbolParsers["whitespace"]), FixType(() => _symbolParsers["all_whitespace"]))))},
			{"ignore_all_whitespace", Symbol("ignore_all_whitespace", Any(Literal(""), FixType(() => _symbolParsers["all_whitespace"])))},
			{"anychar", Symbol("anychar", Literal("*"))},
			{"range", Symbol("range", InOrder(Literal("["), Any(Literal("\\]"), Literal("\\-"), InOrder(Negate(Any(Literal("-"), Literal("]"))), AnyChar())), Literal("-"), Any(Literal("\\]"), Literal("\\-"), InOrder(Negate(Any(Literal("-"), Literal("]"))), AnyChar())), Literal("]")))},
			{"allchars_not_quote", Symbol("allchars_not_quote", InOrder(Negate(Any(Literal("'"), InOrder(FixType(() => _symbolParsers["escaped"]), FixType(() => _symbolParsers["almost_all_escape_chars"]), Literal("\\'")))), Any(InOrder(Negate(Negate(FixType(() => _symbolParsers["escaped"]))), FixType(() => _symbolParsers["all_escape_chars"]), AnyChar()), InOrder(Negate(FixType(() => _symbolParsers["escaped"])), FixType(() => _symbolParsers["all_escape_chars"])), AnyChar()), Any(Literal(""), FixType(() => _symbolParsers["allchars_not_quote"]))))},
			{"literal", Symbol("literal", InOrder(Literal("'"), Any(Literal(""), FixType(() => _symbolParsers["allchars_not_quote"])), Literal("'")))},
			{"allchars_not_gt", Symbol("allchars_not_gt", InOrder(Negate(Any(Literal(">"), InOrder(FixType(() => _symbolParsers["escaped"]), FixType(() => _symbolParsers["almost_all_escape_chars"]), Literal("\\>")))), Any(InOrder(Negate(Negate(FixType(() => _symbolParsers["escaped"]))), FixType(() => _symbolParsers["all_escape_chars"]), AnyChar()), InOrder(Negate(FixType(() => _symbolParsers["escaped"])), FixType(() => _symbolParsers["all_escape_chars"])), AnyChar()), Any(Literal(""), FixType(() => _symbolParsers["allchars_not_gt"]))))},
			{"symbol", Symbol("symbol", InOrder(Literal("<"), FixType(() => _symbolParsers["allchars_not_gt"]), Literal(">")))},
			{"inorder_ele", Symbol("inorder_ele", InOrder(FixType(() => _symbolParsers["ignore_all_whitespace"]), Any(InOrder(FixType(() => _symbolParsers["symbol"]), FixType(() => _symbolParsers["ignore_all_whitespace"]), Negate(Literal("="))), FixType(() => _symbolParsers["literal"]), FixType(() => _symbolParsers["group"]), FixType(() => _symbolParsers["negation"]), FixType(() => _symbolParsers["anychar"]), FixType(() => _symbolParsers["range"]))))},
			{"inorder", Symbol("inorder", Any(FixType(() => _symbolParsers["inorder_ele"]), InOrder(FixType(() => _symbolParsers["inorder_ele"]), FixType(() => _symbolParsers["inorder"]))))},
			{"all_inorder", Symbol("all_inorder", FixType(() => _symbolParsers["inorder"]))},
			{"any", Symbol("any", Any(FixType(() => _symbolParsers["all_inorder"]), InOrder(FixType(() => _symbolParsers["all_inorder"]), Literal("|"), FixType(() => _symbolParsers["any"]))))},
			{"all_any", Symbol("all_any", FixType(() => _symbolParsers["any"]))},
			{"group", Symbol("group", InOrder(Literal("("), FixType(() => _symbolParsers["all_any"]), FixType(() => _symbolParsers["ignore_all_whitespace"]), Literal(")")))},
			{"negation", Symbol("negation", InOrder(Literal("-"), Any(FixType(() => _symbolParsers["group"]), FixType(() => _symbolParsers["symbol"]), FixType(() => _symbolParsers["literal"]), FixType(() => _symbolParsers["negation"]), FixType(() => _symbolParsers["range"]), FixType(() => _symbolParsers["anychar"]))))},
			{"assignment", Symbol("assignment", InOrder(FixType(() => _symbolParsers["ignore_all_whitespace"]), Literal("<"), FixType(() => _symbolParsers["allchars_not_gt"]), Literal(">"), FixType(() => _symbolParsers["ignore_all_whitespace"]), Literal("="), FixType(() => _symbolParsers["all_any"]), FixType(() => _symbolParsers["ignore_all_whitespace"])))},
			{"fallback_point", Symbol("fallback_point", InOrder(Literal("f"), FixType(() => _symbolParsers["assignment"])))},
			{"all_assignments", FallbackPoint(Symbol("all_assignments", InOrder(Any(FixType(() => _symbolParsers["assignment"]), FixType(() => _symbolParsers["fallback_point"])), Any(Literal(""), FixType(() => _symbolParsers["all_assignments"])))))},
			{"all_all_assignments", Symbol("all_all_assignments", FixType(() => _symbolParsers["all_assignments"]))},
			{"EntryPoint", Symbol("EntryPoint", FixType(() => _symbolParsers["all_all_assignments"]))}
        };

        static Parser() {
            _interpreter.Setup(_symbolParsers["EntryPoint"], _symbolHandlers);
        }

        public static Tuple<Parse, List<Error>> Execute(string description) {
            var results = _interpreter.Execute(description);
            return Tuple.Create(Eval((Union<Parse, Func<Parse>>)results.Item1[0]), results.Item2);
        }

        private static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {
            return (data, charsHandledSoFar, acc, errors) => {
                var newAcc = new Result();
                var newErrors = new List<Error>();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, newErrors);

                if(result.Item1 == State.Success) {
                    acc.Add(Tuple.Create(symbolName, newAcc));
                }

                if(newErrors.Count > 0) {
                    errors.Add(new Error($"Failed to parse {symbolName}", newErrors, charsHandledSoFar));
                }

                return result;
            };
        }

        private static Parse Range(char start, char end) {
            return (data, charsHandledSoFar, acc, errors) => {
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome) return emptyCheck.Value;

                if(Methods.IsBetween_Inclusive(start, end, data[0])) {
                    acc.Add(data[0].ToString());
                    return Tuple.Create(State.Success, data.Substring(1), charsHandledSoFar + 1);
                } else {
                    errors.Add(new Error($"Expected a character in the range [{start}-{end}]", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }
            };
        }

        private static Parse Literal(string value) {
            return (data, charsHandledSoFar, acc, errors) => {
                if(data.StartsWith(value)) {
                    acc.Add(value);
                    return Tuple.Create(State.Success, data.Remove(0, value.Length), charsHandledSoFar + value.Length);
                } else {
                    errors.Add(new Error($"Expected {value}", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }
            };
        }

        private static Parse AnyChar() {
            return (data, charsHandledSoFar, acc, errors) => {
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome) return emptyCheck.Value;
                acc.Add(data[0].ToString());
                return Tuple.Create(State.Success, data.Substring(1), charsHandledSoFar + 1);
            };
        }

        private static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {
            return (data, charsHandledSoFar, acc, errors) => {
                var result = Eval(parserToNegate)(data, charsHandledSoFar, new Result(), new List<Error>());
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data, charsHandledSoFar);
                return Tuple.Create(State.Success, data, charsHandledSoFar);
            };
        }

        private static Parse FallbackPoint(Union<Parse, Func<Parse>> parser) {
            return (data, charsHandledSoFar, acc, errors) => {
                var newAcc = new Result();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, errors);
                while(result.Item1 == State.Failure) {
                    if(data.Length == 0) break;
                    data = data.Substring(1); //while the parser failed, skip a character and try again
                    result = Eval(parser)(data, charsHandledSoFar + 1, newAcc = new Result(), new List<Error>());
                }
                if(result.Item1 == State.Success) acc.AddRange(newAcc);
                return result;
            };
        }

        private static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, charsHandledSoFar, acc, errors) => {
                var newAcc = new Result();
                var result = Eval(parsers[0])(data, charsHandledSoFar, newAcc, errors);
                for(int i = 1; i < parsers.Length && result.Item1 == State.Success; ++i) {
                    result = Eval(parsers[i])(result.Item2, result.Item3, newAcc, errors);
                }
                if(result.Item1 == State.Success) acc.AddRange(newAcc);
                else result = Tuple.Create(State.Failure, data, charsHandledSoFar);
                return result;
            };
        }

        private static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, charsHandledSoFar, acc, errors) => {
                var newErrors = new List<Error>();
                var result = parsers.Select(
                    x => {
                        var newAcc = new Result();
                        return new { result = Eval(x)(data, charsHandledSoFar, newAcc, newErrors), acc = newAcc };
                    }
                ).Where(x => x.result.Item1 == State.Success).ToArray();

                if(result.Length == 0) {
                    errors.Add(new Error("All of the following possibilities failed", newErrors, charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }

                var longestMatch = result.MaxBy(x => x.result.Item3);
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

        private static Option<Tuple<State, string, int>> CannotBeEmpty(string data, int charsHandledSoFar, List<Error> errors) {
            if(data.Length > 0) return new None();
            errors.Add(new Error($"No text to consume", charsHandledSoFar));
            return Tuple.Create(State.Failure, data, charsHandledSoFar);
        }
    }
}