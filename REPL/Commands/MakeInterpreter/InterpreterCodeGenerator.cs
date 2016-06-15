using System;
using System.Collections.Generic;
using System.Linq;

namespace REPL.MakeInterpreter {
    public static class InterpreterCodeGenerator {
        public static string LanguageName { get; set; }

        public static string Range(char start, char end) => $"Parser.Range('{start}', '{end}')";
        public static string AnyChar() => $"Parser.AnyChar()";
        public static string Literal(string value) => $"Parser.Literal(\"{value}\")";
        public static string Symbol(string symbolName) => $"Parser.FixType(() => _symbolParsers[\"{symbolName}\"])";
        public static string Negate(string parserToNegate) => $"Parser.Negate({parserToNegate})";
        public static string InOrder(IEnumerable<string> parsers) => $"Parser.InOrder({string.Join(", ", parsers)})";
        public static string Any(IEnumerable<string> parsers) => $"Parser.Any({string.Join(", ", parsers)})";
        public static string Assignment(string symbolName, string parser) => $"Parser.Symbol(\"{symbolName}\", {parser})";
        public static string FallbackPoint(string parser) => $"Parser.FallbackPoint({parser})";

        public static IEnumerable<string> AllAssignments(Tuple<string, string>[] allAssignments) {
            return allAssignments.Select(x => $"{{\"{x.Item1}\", {x.Item2}}}");
        }

        public static string[] EntryPoint(IEnumerable<string> allAssignments) {
            return new[] {
$@"//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicInterpreter {{
    public static partial class {LanguageName}Interpreter {{
        static IReadOnlyDictionary<string, Parse> _symbolParsers = new Dictionary<string, Parse>() {{
            {string.Join(",\n\t\t\t", allAssignments.ToArray())}
        }};

        public static Tuple<List<object>, List<Error>> Execute(string code) {{
            var parserResult = new Result();
            var errors = new List<Error>();
            _symbolParsers[""EntryPoint""](code, 0, parserResult, errors);
            return Tuple.Create(Interpreter.RecursiveEval(parserResult, _symbolHandlers.ToDictionary(x => x.SymbolName)), errors);
        }}
    }}
}}",
$@"//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicInterpreter {{
    public static partial class {LanguageName}Interpreter {{
        static ISymbolHandler[] _symbolHandlers = new ISymbolHandler[] {{
            //////ADD HANDLERS HERE//////
            //new GenericSymbolHandler("""", args => {{
            //        return args;
            //}}),
            //////ADD HANDLERS HERE//////
        }};
    }}
}}",
$@"//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {{
    #region Symbol handlers
    public interface ISymbolHandler {{
        string SymbolName {{ get; }}
        List<object> Call(List<object> args);
    }}

    public class GenericSymbolHandler : ISymbolHandler {{
        Func<List<object>, List<object>> _call;
        public GenericSymbolHandler(string name, Func<List<object>, List<object>> call) {{
            SymbolName = name;
            _call = call;
        }}

        public string SymbolName {{ get; }}
        public virtual List<object> Call(List<object> args) {{ return _call(args); }}
    }}

    public class CombineToStringSymbolHandler : GenericSymbolHandler {{
        public CombineToStringSymbolHandler(string name) : base(name, x => new List<object>() {{ x.ToDelimitedString("""") }}) {{ }}
    }}

    public class IgnoreSymbolHandler : GenericSymbolHandler {{
        public IgnoreSymbolHandler(string name) : base(name, x => new List<object>()) {{ }}
    }}
    #endregion

    #region Interpreter data structures
    public class Error {{
        public Error(string message, int characterIndex) {{
            Message = message;
            CharacterIndex = characterIndex;
        }}

        public Error(string message, List<Error> subErrors, int characterIndex) : this(message, characterIndex) {{
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
    public delegate Tuple<State, string, int> Parse(string data, int charsHandledSoFar, Result acc, List<Error> errors);
    #endregion

    #region Interpreter methods and helper functions
    public static class Interpreter {{
        public static List<object> RecursiveEval(Result result, Dictionary<string, ISymbolHandler> handlers) {{
            return result.SelectMany(
                x => x.Match<List<object>>(
                    val => new List<object>() {{ val }},
                    def => {{
                        var maybeHandler = handlers.TryGetValue(def.Item1);
                        return maybeHandler.IsSome ? maybeHandler.Value.Call(RecursiveEval(def.Item2, handlers)) : RecursiveEval(def.Item2, handlers);
                    }}
                )
            ).ToList();
        }}
    }}
    #endregion

    #region Parse methods and helper functions
    public static class Parser {{
        public static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newAcc = new Result();
                var newErrors = new List<Error>();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, newErrors);

                if(result.Item1 == State.Success) {{
                    acc.Add(Tuple.Create(symbolName, newAcc));
                }}

                if(newErrors.Count > 0) {{
                    errors.Add(new Error($""Failed to parse {{symbolName}}"", newErrors, charsHandledSoFar));
                }}

                return result;
            }};
        }}

        public static Parse Range(char start, char end) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome) return emptyCheck.Value;

                if(Methods.IsBetween_Inclusive(start, end, data[0])) {{
                    acc.Add(data[0].ToString());
                    return Tuple.Create(State.Success, data.Substring(1), charsHandledSoFar + 1);
                }} else {{
                    errors.Add(new Error($""Expected a character in the range [{{start}}-{{end}}]"", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }}
            }};
        }}

        public static Parse Literal(string value) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                if(data.StartsWith(value)) {{
                    acc.Add(value);
                    return Tuple.Create(State.Success, data.Remove(0, value.Length), charsHandledSoFar + value.Length);
                }} else {{
                    errors.Add(new Error($""Expected {{value}}"", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }}
            }};
        }}

        public static Parse AnyChar() {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome) return emptyCheck.Value;
                acc.Add(data[0].ToString());
                return Tuple.Create(State.Success, data.Substring(1), charsHandledSoFar + 1);
            }};
        }}

        public static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var result = Eval(parserToNegate)(data, charsHandledSoFar, new Result(), new List<Error>());
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data, charsHandledSoFar);
                return Tuple.Create(State.Success, data, charsHandledSoFar);
            }};
        }}

        public static Parse FallbackPoint(Union<Parse, Func<Parse>> parser) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newAcc = new Result();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, errors);
                while(result.Item1 == State.Failure) {{
                    if(data.Length == 0) break;
                    data = data.Substring(1); //while the parser failed, skip a character and try again
                    result = Eval(parser)(data, charsHandledSoFar + 1, newAcc = new Result(), new List<Error>());
                }}
                if(result.Item1 == State.Success) acc.AddRange(newAcc);
                return result;
            }};
        }}

        public static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {{
            return (data, charsHandledSoFar, acc, errors) => {{
                var newAcc = new Result();
                var result = Eval(parsers[0])(data, charsHandledSoFar, newAcc, errors);
                for(int i = 1; i < parsers.Length && result.Item1 == State.Success; ++i) {{
                    result = Eval(parsers[i])(result.Item2, result.Item3, newAcc, errors);
                }}
                if(result.Item1 == State.Success) acc.AddRange(newAcc);
                else result = Tuple.Create(State.Failure, data, charsHandledSoFar);
                return result;
            }};
        }}

        public static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {{
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
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }}

                var longestMatch = result.MaxBy(x => x.result.Item3);
                acc.AddRange(longestMatch.acc);
                return longestMatch.result;
            }};
        }}

        public static Parse Eval(Union<Parse, Func<Parse>> val) => val.Match<Parse>(x => x, x => x());
        public static Func<Parse> FixType(Func<Parse> parse) => parse;

        private static Option<Tuple<State, string, int>> CannotBeEmpty(string data, int charsHandledSoFar, List<Error> errors) {{
            if(data.Length > 0) return new None();
            errors.Add(new Error($""No text to consume"", charsHandledSoFar));
            return Tuple.Create(State.Failure, data, charsHandledSoFar);
        }}
    }}
    #endregion
}}"};
        }
    }
}
