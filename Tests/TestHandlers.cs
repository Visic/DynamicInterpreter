using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicInterpreter;
using Utility;

namespace Tests {
    public class Handler : ISymbolHandler {
        protected Func<List<object>, List<object>> _handler;
        protected Handler() { }

        public string SymbolName { get; private set; }

        public List<object> Call(List<object> args) {
            return _handler(args);
        }

        public static Handler Create(string symbolName, Func<List<object>, List<object>> handler) {
            var result = new Handler();
            result.SymbolName = symbolName;
            result._handler = handler;
            return result;
        }

        public static Handler<T> Create<T>(string symbolName, Func<List<object>, T> handler) {
            var result = new Handler<T>();
            result.SymbolName = symbolName;
            result._handler = args => {
                result.LastResult = new List<T>() { handler(args) };
                return result.LastResult.Select(y => (object)y).ToList();
            };
            return result;
        }
    }

    public class Handler<T> : Handler {
        public List<T> LastResult { get; set; } = new List<T>();
    }
}