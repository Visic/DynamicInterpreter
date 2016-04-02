using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public class Interpreter {
        Dictionary<string, SymbolParser> _parsers;
        string _leftOverCode;
        Dictionary<string, ISymbolHandler> _handlers;

        public Option<string> Setup(string grammar, params ISymbolHandler[] handlers) {
            _handlers = handlers.ToDictionary(x => x.SymbolName);
            return CodeParserGenerator.GenerateParsers(grammar).Match(
                defs => { _parsers = defs; },
                err => err
            );
        }

        public List<Option<object>> Execute(string code) {
            var parserResult = new Parser.Result();
            _leftOverCode = _parsers[Constants.EntryPointSymbolName].Parse(_leftOverCode + code, parserResult);
            return RecursiveEval(parserResult);
        }

        private List<Option<object>> RecursiveEval(Parser.Result result) {
            return result.SelectMany(
                x => x.Match<List<Option<object>>>(
                    val => new List<Option<object>>() { new Option<object>(val) },
                    def => {
                        var maybeHandler = _handlers.TryGetValue(def.Item1);
                        return maybeHandler.IsSome ? maybeHandler.Value.Call(RecursiveEval(def.Item2)) : RecursiveEval(def.Item2);
                    }
                )
            ).ToList();
        }
    }
}
