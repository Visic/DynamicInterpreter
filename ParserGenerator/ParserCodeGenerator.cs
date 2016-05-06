using System.Collections.Generic;
using System.Linq;

namespace ParserGenerator {
    public static class ParserCodeGenerator {
        public static string Literal(string value) => $"Literal(\"{value}\")";
        public static string Symbol(string symbolName) => $"FixType(() => SymbolParsers[\"{symbolName}\"])";
        public static string Negate(string parserToNegate) => $"Negate({parserToNegate})";
        public static string InOrder(IEnumerable<string> parsers) => $"InOrder({string.Join(", ", parsers)})";
        public static string Any(IEnumerable<string> parsers) => $"Any({string.Join(", ", parsers)})";
        public static string Assignment(string symbolName, string parser) => $"{{\"{symbolName}\", Symbol(\"{symbolName}\", {parser})}}";

        public static string EntryPoint(IEnumerable<string> allAssignments) {
            return
$@"using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DynamicInterpreter {{
    public static class Parser {{
        public enum State {{ Success, Failure }}
        public class Result : List<Union<string, Tuple<string, Result>>> {{ }}
        public delegate Tuple<State, string> Parse(string data, Result acc);

        public static IReadOnlyDictionary<string, Parse> SymbolParsers = new Dictionary<string, Parse>(){{
            {string.Join(",\n\t\t\t", allAssignments.ToArray())}
        }};

        private static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {{
            return (data, acc) => {{
                var newAcc = new Result();
                var result = Eval(parser)(data, newAcc);
                if (result.Item1 == State.Success) acc.Add(Tuple.Create(symbolName, newAcc));
                return result;
            }};
        }}

        private static Parse Literal(string value) {{
            return (data, acc) => {{
                if(data.StartsWith(value)) {{
                    acc.Add(value);
                    return Tuple.Create(State.Success, data.Remove(0, value.Length));
                }}
                return Tuple.Create(State.Failure, data);
            }};
        }}

        private static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {{
            return (data, acc) => {{
                var newAcc = new Result();
                var result = Eval(parserToNegate)(data, newAcc);
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data);
                return Tuple.Create(State.Success, data);
            }};
        }}

        private static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {{
            return (data, acc) => {{
                var newAcc = new Result();
                var result = data;
                foreach (var parser in parsers) {{
                    var partialResult = Eval(parser)(result, newAcc);
                    if (partialResult.Item1 == State.Failure) return Tuple.Create(State.Failure, data);
                    result = partialResult.Item2;
                }}
                acc.AddRange(newAcc);
                return Tuple.Create(State.Success, result);
            }};
        }}

        private static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {{
            return (data, acc) => {{
                var result = parsers.Select(
                    x => {{
                        var newAcc = new Result();
                        return new {{ result = Eval(x)(data, newAcc), acc = newAcc }};
                    }}
                ).Where(x => x.result.Item1 == State.Success).ToArray();

                if(result.Length == 0) return Tuple.Create(State.Failure, data);
                var longestMatch = result.MinBy(x => x.result.Item2.Length);
                acc.AddRange(longestMatch.acc);
                return longestMatch.result;
            }};
        }}

        private static Parse Eval(Union<Parse, Func<Parse>> val) {{
            return val.Match<Parse>(x => x, x => x());
        }}

        private static Func<Parse> FixType(Func<Parse> parse) {{
            return parse;
        }}
    }}
}}";
        }
    }
}
