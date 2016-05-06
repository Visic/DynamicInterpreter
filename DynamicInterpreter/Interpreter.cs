using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public class Interpreter {
        Parser.Parse _parser;
        Dictionary<string, ISymbolHandler> _handlers;

        public void Setup(Parser.Parse parser, params ISymbolHandler[] handlers) {
            _handlers = handlers.ToDictionary(x => x.SymbolName);
            _parser = parser;
        }

        public Union<List<object>, string> Execute(string code) {
            var parserResult = new Parser.Result();
            var result = _parser(code, parserResult);
            if(result.Item1 == Parser.State.Failure) return "Parse error";
            return RecursiveEval(parserResult);
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
