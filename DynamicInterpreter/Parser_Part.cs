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
                new GenericSymbolHandler("EntryPoint", args => new List<object> { new Union<Parse, Func<Parse>>(assignedSymbols["EntryPoint"]) }),

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
                        assignedSymbols[ele.Item1] = Eval(ele.Item2);
                    }
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
                    return new List<object> { new Union<Parse, Func<Parse>>(Literal(((string)args[1]).Replace("\\\\", "\\"))) };
                })
            );
        }

        public static Tuple<Parse, List<Error>> GenerateParser(string description) {
            var results = _interpreter.Execute(description);
            return Tuple.Create(Eval((Union<Parse, Func<Parse>>)results.Item1[0]), results.Item2);
        }
    }
}
