using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public abstract class Parser {
        public class Result : List<Union<string, Tuple<string, Result>>> { }
        public abstract string Parse(string data, Result acc);
    }

    //this parser parses the symbol's definition
    public class SymbolParser : Parser {
        Lazy<Parser> _parser;
        string _name;

        public SymbolParser(SymbolDefinition def, Dictionary<string, SymbolParser> symbolParsers) {
            _name = def.Name;
            _parser = new Lazy<Parser>(
                () => new AnyParser(
                    def.Definition.Split(x => x.TokenType == Token.Type.Bar)
                                  .Select(
                                      x => new InOrderParser(
                                          x.Select<Token, Parser>(
                                              y => {
                                                  switch (y.TokenType) {
                                                      case Token.Type.Symbol:
                                                          return symbolParsers[y.Value];
                                                      case Token.Type.Literal:
                                                          return new LiteralParser(y.Value);
                                                  }
                                                  throw new Exception("Unknown token type");
                                              }
                                          ).ToArray()
                                      )
                                  ).ToArray()
                )
            );
        }

        public override string Parse(string data, Result acc) {
            var newAcc = new Result();
            var result = _parser.Value.Parse(data, newAcc);
            if (result.Length < data.Length) acc.Add(Tuple.Create(_name, newAcc));
            return result;
        }
    }

    public class LiteralParser : Parser {
        string _literal;

        public LiteralParser(string literal) {
            _literal = literal;
        }

        public override string Parse(string data, Result acc) {
            if (data.StartsWith(_literal)) data = data.Remove(0, _literal.Length);
            acc.Add(_literal);
            return data;
        }
    }

    public class InOrderParser : Parser {
        Parser[] _parsers;

        public InOrderParser(params Parser[] parsers) { _parsers = parsers; }

        public override string Parse(string data, Result acc) {
            var newAcc = new Result();
            var result = data;
            foreach (var parser in _parsers) {
                var partialResult = parser.Parse(result, newAcc);
                if (partialResult.Length == result.Length) return data;
                result = partialResult;
            }
            acc.AddRange(newAcc);
            return result;
        }
    }

    //takes longest match
    public class AnyParser : Parser {
        Parser[] _parsers;

        public AnyParser(params Parser[] parsers) { _parsers = parsers; }

        public override string Parse(string data, Result acc) {
            var result = _parsers.Select(
                                      x => {
                                          var newAcc = new Result();
                                          return new { result = x.Parse(data, newAcc), acc = newAcc };
                                      }
                                  ).MinBy(x => x.result.Length);
            acc.AddRange(result.acc);
            return result.result;
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