using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserGenerator {
    public static class ParserCodeGenerator {
        public static string Literal(string value) => $"Literal(\"{value}\")";
        public static string Symbol(string symbolName) => $"FixType(() => SymbolParsers[\"{symbolName}\"])";
        public static string Negate(string parserToNegate) => $"Negate({parserToNegate})";
        public static string InOrder(IEnumerable<string> parsers) => $"InOrder({string.Join(", ", parsers)})";
        public static string Any(IEnumerable<string> parsers) => $"Any({string.Join(", ", parsers)})";
        public static string Assignment(string symbolName, string parser) => $"Symbol(\"{symbolName}\", {parser})";
        public static string FallbackPoint(string parser) => $"FallbackPoint({parser})";

        public static IEnumerable<string> AllAssignments(Tuple<string, string>[] allAssignments) {
            return allAssignments.Select(x => $"{{\"{x.Item1}\", {x.Item2}}}");
        }

        public static string EntryPoint(IEnumerable<string> allAssignments) {
            return
$@"using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DynamicInterpreter {{
    public static class Parser {{
        public class Error {{
            public Error(string message, int characterIndex) {{
                Message = message;
                CharacterIndex = characterIndex;
            }}

            public Error(string message, List<Error> subErrors, int characterIndex) : this (message, characterIndex) {{
                SubErrors = subErrors;
            }}

            public string Message {{ get; }}
            public int CharacterIndex {{ get; }}
            public List<Error> SubErrors {{ get; }} = new List<Error>();

            public override string ToString() {{
                return $""Index {{CharacterIndex}}: {{Message}}"";
            }}
        }}

        public enum State {{ Success, Failure }}
        public class Result : List<Union<string, Tuple<string, Result>>> {{ }}
        public delegate Tuple<State, string> Parse(string data, int charsHandledSoFar, Result acc, List<Error> errors);

        public static IReadOnlyDictionary<string, Parse> SymbolParsers = new Dictionary<string, Parse>() {{
            {string.Join(",\n\t\t\t", allAssignments.ToArray())}
        }};

        private static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newAcc = new Result();
                var newErrors = new List<Error>();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, newErrors);

                if(result.Item1 == State.Success) {{
                    acc.Add(Tuple.Create(symbolName, newAcc));
                }}

                if (newErrors.Count > 0) {{
                    errors.Add(new Error($""Failed to parse {{symbolName}}"", newErrors, charsHandledSoFar));
                }}

                return result;
            }};
        }}

        private static Parse Literal(string value) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                if(data.StartsWith(value)) {{
                    acc.Add(value);
                    return Tuple.Create(State.Success, data.Remove(0, value.Length));
                }} else {{
                    errors.Add(new Error($""Expected {{value}}"", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data);
                }}
            }};
        }}

        private static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var result = Eval(parserToNegate)(data, charsHandledSoFar, new Result(), new List<Error>());
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data);
                return Tuple.Create(State.Success, data);
            }};
        }}

        private static Parse FallbackPoint(Union<Parse, Func<Parse>> parser) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newAcc = new Result();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, errors);
                while(result.Item1 == State.Failure) {{
                    if(data.Length == 0) break;
                    data = data.Substring(1); //while the parser failed, skip a character and try again
                    result = Eval(parser)(data, charsHandledSoFar, newAcc = new Result(), new List<Error>());
                }}
                if(result.Item1 == State.Success) acc.AddRange(newAcc);
                return result;
            }};
        }}

        private static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newAcc = new Result();
                var result = data;
                foreach(var parser in parsers) {{
                    var partialResult = Eval(parser)(result, charsHandledSoFar, newAcc, errors);
                    if(partialResult.Item1 == State.Failure) return Tuple.Create(State.Failure, data);
                    result = partialResult.Item2;
                }}
                acc.AddRange(newAcc);
                return Tuple.Create(State.Success, result);
            }};
        }}

        private static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newErrors = new List<Error>();
                var result = parsers.Select(
                    x => {{
                        var newAcc = new Result();
                        return new {{ result = Eval(x)(data, charsHandledSoFar, newAcc, newErrors), acc = newAcc }};
                    }}
                ).Where(x => x.result.Item1 == State.Success).ToArray();

                if(result.Length == 0) {{
                    errors.Add(new Error(""All of the following possibilities failed"", newErrors, charsHandledSoFar));
                    return Tuple.Create(State.Failure, data);
                }}

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
