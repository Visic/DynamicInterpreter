using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public abstract class Parser {
        public enum State { Success, Failure }

        public class Result : List<Union<string, Tuple<string, Result>>> { }
        public abstract Tuple<State, string> Parse(string data, Result acc);
    }

    //this parser parses the symbol's definition
    public class SymbolParser : Parser {
        Lazy<Parser> _parser;
        string _name;

        public SymbolParser(SymbolDefinition def, Dictionary<string, SymbolParser> symbolParsers) {
            _name = def.Name;
            _parser = new Lazy<Parser>(() => MakeParser(def.Definition.ToList(), symbolParsers).Value);
        }

        public override Tuple<State, string> Parse(string data, Result acc) {
            var newAcc = new Result();
            var result = _parser.Value.Parse(data, newAcc);
            if (result.Item1 == State.Success) acc.Add(Tuple.Create(_name, newAcc));
            return result;
        }

        private Option<Parser> MakeParser(List<Token> def, Dictionary<string, SymbolParser> symbolParsers) {
            var defParsers = new List<Parser>();
            var curParsers = new List<Parser>();
            var negativeMatch = 0;

            Action handleCurParsers = () => {
                if(curParsers.Count > 1) defParsers.Add(new InOrderParser(curParsers.ToArray()));
                else if(curParsers.Count == 1) defParsers.Add(curParsers[0]);
                curParsers.Clear();
            };

            bool ret = false;
            while(def.Count > 0 && !ret) {
                var token = def[0];
                def.RemoveAt(0);

                var relevantParser = new Option<Parser>();
                switch(token.TokenType) {
                    case Token.Type.CloseGroup:
                        ret = true;
                        break;
                    case Token.Type.Bar:
                        handleCurParsers();
                        break;
                    case Token.Type.Symbol:
                        relevantParser = symbolParsers[token.Value];
                        break;
                    case Token.Type.Literal:
                        relevantParser = new LiteralParser(token.Value);
                        break;
                    case Token.Type.NegativeMatch:
                        negativeMatch++;
                        break;
                    case Token.Type.OpenGroup:
                        relevantParser = MakeParser(def, symbolParsers);
                        break;
                    default:
                        throw new Exception("Unknown token type");
                }

                relevantParser.Apply(p => {
                    for(; negativeMatch > 0; --negativeMatch) {
                        p = new NegativeMatchParser(p);
                    }

                    curParsers.Add(p);
                });
            }
            handleCurParsers();
            return defParsers.Count > 1 ? new AnyParser(defParsers.ToArray()) : defParsers[0];
        }
    }

    public class NegativeMatchParser : Parser {
        Parser _parserToNegate;
        public NegativeMatchParser(Parser parserToNegate) { _parserToNegate = parserToNegate; }

        public override Tuple<State, string> Parse(string data, Result acc) {
            var newAcc = new Result();
            var result = _parserToNegate.Parse(data, newAcc);
            if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data);
            return Tuple.Create(State.Success, data);
        }
    }

    public class LiteralParser : Parser {
        string _literal;

        public LiteralParser(string literal) {
            _literal = literal;
        }

        public override Tuple<State, string> Parse(string data, Result acc) {
            if(data.StartsWith(_literal)) {
                acc.Add(_literal);
                return Tuple.Create(State.Success, data.Remove(0, _literal.Length));
            }
            return Tuple.Create(State.Failure, data);
        }
    }

    public class InOrderParser : Parser {
        Parser[] _parsers;

        public InOrderParser(params Parser[] parsers) { _parsers = parsers; }

        public override Tuple<State, string> Parse(string data, Result acc) {
            var newAcc = new Result();
            var result = data;
            foreach (var parser in _parsers) {
                var partialResult = parser.Parse(result, newAcc);
                if (partialResult.Item1 == State.Failure) return Tuple.Create(State.Failure, data);
                result = partialResult.Item2;
            }
            acc.AddRange(newAcc);
            return Tuple.Create(State.Success, result);
        }
    }

    //takes longest match
    public class AnyParser : Parser {
        Parser[] _parsers;

        public AnyParser(params Parser[] parsers) { _parsers = parsers; }

        public override Tuple<State, string> Parse(string data, Result acc) {
            var result = _parsers.Select(
                                      x => {
                                          var newAcc = new Result();
                                          return new { result = x.Parse(data, newAcc), acc = newAcc };
                                      }
                                  ).Where(x => x.result.Item1 == State.Success).ToArray();

            if(result.Length == 0) return Tuple.Create(State.Failure, data);
            var longestMatch = result.MinBy(x => x.result.Item2.Length);
            acc.AddRange(longestMatch.acc);
            return longestMatch.result;
        }
    }

    public class CodeParserGenerator {
        public static Union<Dictionary<string, SymbolParser>, string> GenerateParsers(string grammar) {
            var symbolParsers = new Dictionary<string, SymbolParser>();
            return GrammarParser.Parse(grammar).Match(
                defs => {
                    Methods.For(
                        defs, 
                        x => symbolParsers[x.Name] = new SymbolParser(x, symbolParsers)
                    );
                    return symbolParsers;
                },
                err => err
            );
        }
    }
}