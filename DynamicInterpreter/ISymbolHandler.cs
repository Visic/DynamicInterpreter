using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public interface ISymbolHandler {
        string SymbolName { get; }
        List<object> Call(List<object> args);
    }
}