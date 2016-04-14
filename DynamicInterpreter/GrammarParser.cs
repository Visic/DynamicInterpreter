using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public class Token {
        public enum Type {
            Symbol,
            Literal,
            NegativeMatch,
            Bar,
            OpenGroup,
            CloseGroup
        }

        public Token(string value, Type tokenType) {
            Value = value;
            TokenType = tokenType;
        }

        public string Value { get; }
        public Type TokenType { get; }
    }

    public class SymbolDefinition {
        public SymbolDefinition(string name, IReadOnlyList<Token> definition) {
            Name = name;
            Definition = definition;
        }

        public string Name { get; }
        public IReadOnlyList<Token> Definition { get; }
    }

    public static class GrammarParser {
        //<symbol name>=<symbol name>|'literal'|<>
        //special characters -<>|='
        //whitespace in <> or '' is part of that (symbol name/literal)
        //  all other whitespace is ignored so you can format your grammar however you'd like

        //Starting char for token -> (Ending char for token, Token type)
        static readonly Dictionary<char, Tuple<char, Token.Type>> _tokenDelims = new Dictionary<char, Tuple<char, Token.Type>>() {
            { '<', Tuple.Create('>', Token.Type.Symbol) },
            { '\'', Tuple.Create('\'', Token.Type.Literal) },
        };

        //return symbol definitions or an error
        public static Union<SymbolDefinition[], string> Parse(string grammar) {
            if (!string.IsNullOrWhiteSpace(grammar)) {
                var symbolDefs = new List<SymbolDefinition>();
                var negativeMatch = false;
                Token curSymbol = null; //the token we are working on defining
                var tokens = new List<Token>();

                while (true) {
                    grammar = grammar.TrimStart();
                    var initialLength = grammar.Length;
                    if (initialLength == 0) break;

                    var ch = grammar[0];
                    grammar = grammar.Substring(1);
                    switch (ch) {
                        case '=':
                            if (curSymbol != null) { //skip for the first symbol, we haven't gathered it's def yet
                                symbolDefs.Add(new SymbolDefinition(curSymbol.Value, tokens.GetRange(0, tokens.Count - 1)));
                            }
                            curSymbol = tokens[tokens.Count - 1];
                            tokens.Clear();
                            break;
                        case '|':
                            tokens.Add(new Token("|", Token.Type.Bar));
                            break;
                        case '-':
                            negativeMatch = true;
                            break;
                        case '(':
                            tokens.Add(new Token("(", Token.Type.OpenGroup));
                            break;
                        case ')':
                            tokens.Add(new Token(")", Token.Type.CloseGroup));
                            break;
                        default:
                            _tokenDelims.TryGetValue(ch).Match(
                                some => {
                                    if(negativeMatch) {
                                        tokens.Add(new Token("-", Token.Type.NegativeMatch));
                                        negativeMatch = false;
                                    }

                                    var valAndRest = ParseToken(grammar, some.Item1);
                                    tokens.Add(new Token(valAndRest.Item1, some.Item2));
                                    grammar = valAndRest.Item2;
                                },
                                none => {
                                    negativeMatch = false;
                                }
                            );
                            break;
                    }
                    if (grammar.Length == initialLength) break; //no characters were handled, we're done
                }

                if (curSymbol != null) symbolDefs.Add(new SymbolDefinition(curSymbol.Value, tokens.GetRange(0, tokens.Count))); //handle the last symbol def
                if (grammar.Length == 0) return symbolDefs.ToArray();
            }

            return "Invalid definition";
        }

        private static Tuple<string, string> ParseToken(string grammar, char endsWith) {
            var parts = grammar.Split(new[] { endsWith }, 2);
            if (parts.Length == 1) return Tuple.Create((string)null, grammar);
            if (parts[0].EndsWith("\\")) {
                var slashCount = 1;
                for(; slashCount < parts[0].Length; ++slashCount) {
                    if (parts[0][parts[0].Length - 1 - slashCount] != '\\') break;
                }

                if (slashCount.IsOdd()) {
                    var res = ParseToken(parts[1], endsWith);
                    parts[0] = $"{parts[0].Remove(parts[0].Length - 1)}{endsWith}{res.Item1}";
                    parts[1] = res.Item2;
                }
            }
            return Tuple.Create(parts[0].Replace(@"\\", @"\"), parts[1]);
        }
    }
}