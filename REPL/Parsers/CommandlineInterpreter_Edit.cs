//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using REPL;
using Utility;

namespace DynamicInterpreter {
    public static partial class CommandlineInterpreter {
        static ISymbolHandler[] _symbolHandlers = new ISymbolHandler[] {
            //////ADD HANDLERS HERE//////
            new CombineToStringSymbolHandler("argname"),
            new IgnoreSymbolHandler("allchars_no_dash"),
            new IgnoreSymbolHandler("argname_value_separator"),
            new IgnoreSymbolHandler("allwhitespace"),
            new CombineToStringSymbolHandler("argvalue"),

            new GenericSymbolHandler("arg", x => {
                ArgsAndSettings._options.TryGetValue(x[1].ToString()).Apply(y => y(x.Skip(2).Select(z => z.ToString().Trim('\"')).ToArray()));
                return new List<object>();
            })
            //////ADD HANDLERS HERE//////
        };
    }
}