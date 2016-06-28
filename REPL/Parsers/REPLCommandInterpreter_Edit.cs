//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using REPL;

namespace DynamicInterpreter {
    public static partial class REPLCommandInterpreter {
        static ISymbolHandler[] _symbolHandlers = new ISymbolHandler[] {
            //////ADD HANDLERS HERE//////
            new CombineToStringSymbolHandler("arg"),
            new CombineToStringSymbolHandler("cmdname"),
            new GenericSymbolHandler("command", x => Program.CallREPLCommand(x))
            //////ADD HANDLERS HERE//////
        };
    }
}