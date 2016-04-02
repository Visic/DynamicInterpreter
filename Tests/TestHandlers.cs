using DynamicInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Tests {
    public class Handler : ISymbolHandler {
        protected Func<List<Option<object>>, List<Option<object>>> _handler;
        protected Handler() { }

        public string SymbolName { get; private set; }

        public List<Option<object>> Call(List<Option<object>> args) {
            return _handler(args);
        }

        public static Handler Create(string symbolName, Func<List<Option<object>>, List<Option<object>>> handler) {
            var result = new Handler();
            result.SymbolName = symbolName;
            result._handler = handler;
            return result;
        }

        public static Handler<T> Create<T>(string symbolName, Func<List<Option<object>>, T> handler) {
            var result = new Handler<T>();
            result.SymbolName = symbolName;
            result._handler = args => {
                result.LastResult = new List<Option<T>>() { new Option<T>(handler(args)) };
                return result.LastResult.Select(y => y.Apply(x => (object)x)).ToList();
            };
            return result;
        }
    }

    public class Handler<T> : Handler {
        public List<Option<T>> LastResult { get; set; } = new List<Option<T>>();
    }
}