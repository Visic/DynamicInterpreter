//AUTO-GENERATED - Dynamic Interpreter (by Andrew Frailing  https://github.com/Visic)
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;

namespace DynamicInterpreter {
    public static partial class Parser {
        static Dictionary<string, Parse> _assignedSymbols = new Dictionary<string, Parse>();

        static ISymbolHandler[] _symbolHandlers = new ISymbolHandler[] {
            //////ADD HANDLERS HERE//////
            new IgnoreSymbolHandler("ignore_all_whitespace"),
            new GenericSymbolHandler("symbol", args => new List<object> { new Union<Parse, Func<Parse>>(FixType(() => _assignedSymbols[(string)args[1]])) }),
            new GenericSymbolHandler("negation", args => new List<object> { new Union<Parse, Func<Parse>>(Negate((Union<Parse, Func<Parse>>)args[1])) }),
            new GenericSymbolHandler("group", args => new List<object> { (Union<Parse, Func<Parse>>)args[1] }),
            new GenericSymbolHandler("EntryPoint", args => new List<object> { new Union<Parse, Func<Parse>>(_assignedSymbols["EntryPoint"]) }),

            new GenericSymbolHandler("allchars_not_gt", args => {
                return new List<object> { args.Cast<string>().ToDelimitedString("").Replace(@"\>", ">") };
            }),

            new GenericSymbolHandler("allchars_not_quote", args => {
                return new List<object> { args.Cast<string>().ToDelimitedString("").Replace(@"\'", "'") };
            }),

            new GenericSymbolHandler("fallback_point", args => {
                var assignment = (Tuple<string, Union<Parse, Func<Parse>>>)args[1];
                return new List<object> { Tuple.Create(assignment.Item1, new Union<Parse, Func<Parse>>(FallbackPoint(assignment.Item2))) };
            }),

            new GenericSymbolHandler("assignment", args => {
                var str = ((string)args[1]).Replace("\\\\", "\\");
                return new List<object>() { Tuple.Create(str, new Union<Parse, Func<Parse>>(Symbol(str, (Union<Parse, Func<Parse>>)args[4]))) };
            }),

            new GenericSymbolHandler("all_all_assignments", args => {
                var castArgs = args.Take(args.Count - 1).Cast<Tuple<string, Union<Parse, Func<Parse>>>>().ToArray();
                foreach(var ele in castArgs) {
                    _assignedSymbols[ele.Item1] = Eval(ele.Item2);
                }
                return new List<object>();
            }),

            new GenericSymbolHandler("all_inorder", args => {
                if(args.Count == 1) return args;
                return new List<object> { new Union<Parse, Func<Parse>>(InOrder(args.Cast<Union<Parse, Func<Parse>>>().ToArray())) };
            }),

            new GenericSymbolHandler("anychar", args => {
                return new List<object> { new Union<Parse, Func<Parse>>(AnyChar()) };
            }),

            new GenericSymbolHandler("range", args => {
                return new List<object> { new Union<Parse, Func<Parse>>(Range(((string)args[1])[0], ((string)args[3])[0])) };
            }),

            new GenericSymbolHandler("all_any", args => {
                if(args.Count == 1) return args;
                return new List<object> { new Union<Parse, Func<Parse>>(Any(args.Where(x => !(x is string)).Cast<Union<Parse, Func<Parse>>>().ToArray())) }; //just use the parsers, ignore the '|' strings
            }),

            new GenericSymbolHandler("literal", args => {
                return new List<object> { new Union<Parse, Func<Parse>>(Literal(((string)args[1]).Replace("\\\\", "\\"))) };
            })
            //////ADD HANDLERS HERE//////
        };
    }
}