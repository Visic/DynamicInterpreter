using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public interface ISymbolHandler {
        string SymbolName { get; }
        List<Option<object>> Call(List<Option<object>> args);
    }
}