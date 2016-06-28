//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    #region Symbol handlers
    public interface ISymbolHandler {
        string SymbolName { get; }
        List<object> Call(List<object> args);
    }

    public class GenericSymbolHandler : ISymbolHandler {
        Func<List<object>, List<object>> _call;
        
        public GenericSymbolHandler(string name, Func<List<object>, List<object>> call) {
            SymbolName = name;
            _call = call;
        }
        
        public GenericSymbolHandler(string name, Func<List<object>, object> call) : this(name, x => new List<object>(){ call(x) }) { }
        public GenericSymbolHandler(string name, Action<List<object>> call) : this(name, x => { call(x); return new List<object>(); }) { }

        public string SymbolName { get; }
        public virtual List<object> Call(List<object> args) { return _call(args); }
    }

    public class CombineToStringSymbolHandler : GenericSymbolHandler {
        public CombineToStringSymbolHandler(string name) : base(name, x => new List<object>() { x.ToDelimitedString("") }) { }
    }

    public class IgnoreSymbolHandler : GenericSymbolHandler {
        public IgnoreSymbolHandler(string name) : base(name, x => new List<object>()) { }
    }
    #endregion

    #region Interpreter data structures
    public class Error {
        public Error(string message, int characterIndex) {
            Message = message;
            CharacterIndex = characterIndex;
        }

        public Error(string message, List<Error> subErrors, int characterIndex) : this(message, characterIndex) {
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
    public class Result : List<Union<Tuple<int, string>, Tuple<int, string, Result>>> { }
    public delegate Tuple<State, string, int> Parse(string data, int charsHandledSoFar, Result acc, List<Error> errors);
    #endregion

    #region Interpreter methods and helper functions
    public static class Interpreter {
        public static Union<List<object>, Error> RecursiveEval(Result parserResult, Dictionary<string, ISymbolHandler> handlers) {
            var result = new List<object>();
            foreach(var ele in parserResult) {
                var partialResultOrError = ele.Match<Union<List<object>, Error>>(
                    val => new List<object>() { val.Item2 },
                    def => RecursiveEval(def.Item3, handlers).Match<Union<List<object>, Error>>(
                        args => handlers.TryGetValue(def.Item2).Match<Union<List<object>, Error>>(
                            handler => {
                                try {
                                    return handler.Call(args);
                                } catch (Exception ex) {
                                    return new Error(ex.Message, def.Item1);
                                }
                            }, 
                            noHandler => args //no handler defined, just return what we have
                        ),
                        err => err //just return the error
                    )
                );

                //if there is an error, return it, otherwise aggregate the results
                if (partialResultOrError.Match(x => result.AddRange(x), x => true).IsSome) return partialResultOrError;
            }
            return result;
        }
    }
    #endregion

    #region Parse methods and helper functions
    public static class Parser {
        public static Parse Symbol(string symbolName, Union<Parse, Func<Parse>> parser) {
            return (data, charsHandledSoFar, acc, errors) => {
                var newAcc = new Result();
                var newErrors = new List<Error>();
                var result = Eval(parser)(data, charsHandledSoFar, newAcc, newErrors);

                if(result.Item1 == State.Success) {
                    acc.Add(Tuple.Create(charsHandledSoFar, symbolName, newAcc));
                }

                if(newErrors.Count > 0) {
                    errors.Add(new Error($"Failed to parse {symbolName}", newErrors, charsHandledSoFar));
                }

                return result;
            };
        }

        public static Parse Repeat(Union<Parse, Func<Parse>> parser, int? start, int? end) {
            return (data, charsHandledSoFar, acc, errors) => {
                start = start ?? 0;
                if (end.HasValue && end < start) {
                    errors.Add(new Error($"End [{end}] is less than start [{start}]", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome && start > 0) return emptyCheck.Value;

                var newAcc = new Result();
                var newErrors = new List<Error>();
                var currentResult = Tuple.Create(State.Success, data, charsHandledSoFar);
                var i = 0;
                for(; i < start; ++i) {
                    currentResult = Eval(parser)(currentResult.Item2, currentResult.Item3, newAcc, newErrors);
                    if(currentResult.Item1 == State.Failure) {
                        errors.Add(new Error($"Failed to consume at least {start} repetitions", newErrors, charsHandledSoFar));
                        return Tuple.Create(State.Failure, data, charsHandledSoFar);
                    }
                }

                var result = currentResult;
                acc.AddRange(newAcc);
                errors.AddRange(newErrors);
                newAcc.Clear();
                newErrors.Clear();
                while(true) {
                    if (end.HasValue && ++i == end) break;
                    currentResult = Eval(parser)(currentResult.Item2, currentResult.Item3, newAcc, newErrors);
                    if(currentResult.Item1 == State.Success) { 
                        result = currentResult;
                        acc.AddRange(newAcc);
                        errors.AddRange(newErrors);
                        newAcc.Clear();
                        newErrors.Clear();
                    } else {
                        break;
                    }
                }
                return result;
            };
        }

        public static Parse Range(char start, char end) {
            return (data, charsHandledSoFar, acc, errors) => {
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome) return emptyCheck.Value;

                if(Methods.IsBetween_Inclusive(start, end, data[0])) {
                    acc.Add(Tuple.Create(charsHandledSoFar, data[0].ToString()));
                    return Tuple.Create(State.Success, data.Substring(1), charsHandledSoFar + 1);
                } else {
                    errors.Add(new Error($"Expected a character in the range [{start}-{end}]", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }
            };
        }

        public static Parse Literal(string value) {
            return (data, charsHandledSoFar, acc, errors) => {
                if(data.StartsWith(value)) {
                    acc.Add(Tuple.Create(charsHandledSoFar, value));
                    return Tuple.Create(State.Success, data.Remove(0, value.Length), charsHandledSoFar + value.Length);
                } else {
                    errors.Add(new Error($"Expected {value}", charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }
            };
        }

        public static Parse AnyChar() {
            return (data, charsHandledSoFar, acc, errors) => {
                var emptyCheck = CannotBeEmpty(data, charsHandledSoFar, errors);
                if(emptyCheck.IsSome) return emptyCheck.Value;
                acc.Add(Tuple.Create(charsHandledSoFar, data[0].ToString()));
                return Tuple.Create(State.Success, data.Substring(1), charsHandledSoFar + 1);
            };
        }

        public static Parse Negate(Union<Parse, Func<Parse>> parserToNegate) {
            return (data, charsHandledSoFar, acc, errors) => {
                var result = Eval(parserToNegate)(data, charsHandledSoFar, new Result(), new List<Error>());
                if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data, charsHandledSoFar);
                return Tuple.Create(State.Success, data, charsHandledSoFar);
            };
        }

        public static Parse FallbackPoint(Union<Parse, Func<Parse>> parser) {
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

        public static Parse InOrder(params Union<Parse, Func<Parse>>[] parsers) {
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

        public static Parse Any(params Union<Parse, Func<Parse>>[] parsers) {
            return (data, charsHandledSoFar, acc, errors) => {
                var result = parsers.Select(
                    x => {
                        var newErrors = new List<Error>();
                        var newAcc = new Result();
                        return new { result = Eval(x)(data, charsHandledSoFar, newAcc, newErrors), acc = newAcc, errors = newErrors };
                    }
                ).ToArray();

                if(result.All(x => x.result.Item1 == State.Failure)) {
                    errors.Add(new Error("All of the following possibilities failed", result.SelectMany(x => x.errors).ToList(), charsHandledSoFar));
                    return Tuple.Create(State.Failure, data, charsHandledSoFar);
                }

                var longestMatch = result.Where(x => x.result.Item1 == State.Success).MaxBy(x => x.result.Item3);
                acc.AddRange(longestMatch.acc);
                errors.AddRange(longestMatch.errors);
                return longestMatch.result;
            };
        }

        public static Parse Eval(Union<Parse, Func<Parse>> val) => val.Match<Parse>(x => x, x => x());
        public static Func<Parse> FixType(Func<Parse> parse) => parse;

        private static Option<Tuple<State, string, int>> CannotBeEmpty(string data, int charsHandledSoFar, List<Error> errors) {
            if(data.Length > 0) return new None();
            errors.Add(new Error($"No text to consume", charsHandledSoFar));
            return Tuple.Create(State.Failure, data, charsHandledSoFar);
        }
    }
    #endregion
}