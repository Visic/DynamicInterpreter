using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public static partial class Parser {
        static Interpreter _interpreter = new Interpreter();
        static Dictionary<string, Union<Parse, Func<Parse>>> _assignedSymbols = new Dictionary<string, Union<Parse, Func<Parse>>>();

        static Parser() {
            _interpreter.Setup(
                Eval(SymbolParsers["EntryPoint"]),

                //Handlers
                new IgnoreSymbolHandler("ignore_all_whitespace"),
                new CombineToStringSymbolHandler("allchars_not_gt"),
                new CombineToStringSymbolHandler("allchars_not_quote"),
                new GenericSymbolHandler("symbol", args => new List<object> { new Union<Parse, Func<Parse>>(FixType(() => Eval(_assignedSymbols[(string)args[1]]))) }),
                new GenericSymbolHandler("negation", args => new List<object> { new Union<Parse, Func<Parse>>(Negate((Union<Parse, Func<Parse>>)args[1])) }),
                new GenericSymbolHandler("group", args => new List<object> { (Union<Parse, Func<Parse>>)args[1] }),
                new GenericSymbolHandler("EntryPoint", args => new List<object> { _assignedSymbols["EntryPoint"] }),

                new GenericSymbolHandler("assignment", args => {
                    _assignedSymbols[(string)args[1]] = Symbol((string)args[1], (Union<Parse, Func<Parse>>)args[4]);
                    return new List<object>();
                }),

                new GenericSymbolHandler("all_inorder", args => {
                    if(args.Count == 1) return args;
                    return new List<object> { new Union<Parse, Func<Parse>>(InOrder(args.Cast<Union<Parse, Func<Parse>>>().ToArray())) };
                }),

                new GenericSymbolHandler("all_any", args => {
                    if(args.Count == 1) return args;
                    return new List<object> { new Union<Parse, Func<Parse>>(Any(args.Where(x => !(x is string)).Cast<Union<Parse, Func<Parse>>>().ToArray())) }; //just use the parsers, ignore the '|' strings
                }),

                new GenericSymbolHandler("literal", args => {
                    var str = (string)args[1];
                    str = str.Replace(@"\'", "'");
                    str = str.Replace("\"", "\\\"");
                    return new List<object> { new Union<Parse, Func<Parse>>(Literal(str)) };
                })
            );
        }

        public static Union<Parse, string> GenerateParser(string description) {
            _assignedSymbols.Clear();

            return _interpreter.Execute(description).Match(
                x => Eval((Union<Parse, Func<Parse>>)x[0]),
                x => x
            );
        }
    }
}
