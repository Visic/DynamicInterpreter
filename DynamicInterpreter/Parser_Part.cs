using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DynamicInterpreter {
    public static partial class Parser {
        static Interpreter _interpreter = new Interpreter();

        static Parser() {
            var assignedSymbols = new Dictionary<string, Parse>();

            _interpreter.Setup(
                SymbolParsers["EntryPoint"],

                //Handlers
                new IgnoreSymbolHandler("ignore_all_whitespace"),
                new GenericSymbolHandler("symbol", args => new List<object> { new Union<Parse, Func<Parse>>(FixType(() => assignedSymbols[(string)args[1]])) }),
                new GenericSymbolHandler("negation", args => new List<object> { new Union<Parse, Func<Parse>>(Negate((Union<Parse, Func<Parse>>)args[1])) }),
                new GenericSymbolHandler("group", args => new List<object> { (Union<Parse, Func<Parse>>)args[1] }),
                new GenericSymbolHandler("EntryPoint", args => new List<object> { assignedSymbols["EntryPoint"] }),

                new GenericSymbolHandler("assignment", args => {
                    var str = ((string)args[1]).Replace("\\\\", "\\");
                    assignedSymbols[str] = Symbol(str, (Union<Parse, Func<Parse>>)args[4]);
                    return new List<object>();
                }),

                new GenericSymbolHandler("allchars_not_gt", args => {
                    return new List<object> { args.Cast<string>().ToDelimitedString("").Replace(@"\>", ">") };
                }),

                new GenericSymbolHandler("allchars_not_quote", args => {
                    return new List<object> { args.Cast<string>().ToDelimitedString("").Replace(@"\'", "'") };
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
                    return new List<object> { new Union<Parse, Func<Parse>>(Literal(((string)args[1]).Replace("\\\\", "\\"))) };
                })
            );
        }

        public static Union<Parse, string> GenerateParser(string description) {
            return _interpreter.Execute(description).Match(
                x => (Parse)x[0],
                x => x
            );
        }
    }
}
