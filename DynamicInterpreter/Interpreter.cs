using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using static DynamicInterpreter.Parser;

namespace DynamicInterpreter {
    public class Interpreter {
        Parse _parser;
        Dictionary<string, ISymbolHandler> _handlers;

        public void Setup(Parse parser, params ISymbolHandler[] handlers) {
            _handlers = handlers.ToDictionary(x => x.SymbolName);
            _parser = parser;
        }

        public Tuple<List<object>, List<Error>> Execute(string code) {
            var parserResult = new Result();
            var errors = new List<Error>();
            var result = _parser(code, parserResult, errors);
            return Tuple.Create(RecursiveEval(parserResult), errors);
        }

        private List<object> RecursiveEval(Parser.Result result) {
            return result.SelectMany(
                x => x.Match<List<object>>(
                    val => new List<object>() { val },
                    def => {
                        var maybeHandler = _handlers.TryGetValue(def.Item1);
                        return maybeHandler.IsSome ? maybeHandler.Value.Call(RecursiveEval(def.Item2)) : RecursiveEval(def.Item2);
                    }
                )
            ).ToList();
        }
    }
}
