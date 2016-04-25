using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicInterpreter;
using ParserGenerator.Properties;
using Utility;

namespace ParserGenerator {
    public class ParseGraph {
        public class Node {
            public bool Negate { get; set; }
            public Node Child { get; set; }
            public HashSet<string> Value { get; set; }
        }

        //TODO:: I think any and inorder might be wrong, and symbols, as they are defined, should get stored in a dictionary from symbol name -> node,
        //then, whenever that symbol name is found, I can reference the same node to get my looping in the graph
        ISymbolHandler[] _handlers = new ISymbolHandler[] {
            new IgnoreSymbolHandler("ignore_all_whitespace"),
            //return the symbol name (removing the <> from start and end)
            new GenericSymbolHandler("symbol", HandleLiteralOrSymbol),
            //return the literal value (removing the ' from start and end)
            new GenericSymbolHandler("literal", HandleLiteralOrSymbol),
            //return a Node who's value is just the hashset of all args
            new GenericSymbolHandler("any", HandleAny),
            //return a Node who's children is each arg in order
            new GenericSymbolHandler("inorder", HandleInOrder),
            //return the Any node, ignore the parens
            new GenericSymbolHandler("group", x => x.Skip(1).Take(x.Count - 2).ToList()),
            
            new GenericSymbolHandler("negate", x => new List<object>() {  }),
        };

        public ParseGraph(string grammar) {
            var interp = new Interpreter();
            var result = interp.Setup(Resources.DescriptionLanguageGrammar);
        }

        private static List<object> HandleLiteralOrSymbol(List<object> args) {
            return new List<object> {
                //return a single string element
                args.Select(y => (string)y)
                    .ToDelimitedString("")
                    .RemoveCount(1, true)
                    .RemoveCount(1, false)
            };
        }

        private static List<object> HandleAny(List<object> args) {
            var strings = new HashSet<string>();
            var nodes = new List<object>();

            Methods.For(
                TransformArgs(args), 
                ele => ele.Match(
                    node => nodes.Add(node), 
                    str => strings.Add(str)
                )
            );
            return nodes.Concat(strings).ToList();
        }

        private static List<object> HandleInOrder(List<object> args) {
            var root = new Node();
            var curChild = root;

            Methods.For(
                TransformArgs(args),
                ele => ele.Match(
                    node => { curChild = curChild.Child = node; },
                    str => { curChild = curChild.Child = new Node() { Value = new HashSet<string>() { str } }; }
                )
            );
            return new List<object>() { root.Child }; //Note:: Return root's child, because root never had any value, it was just a seed for the accumulation
        }

        private static List<Union<Node, string>> TransformArgs(List<object> args) {
            return args.Select<object, Union<Node, string>>(x => {
                if(x is string) return (string)x;
                else return (Node)x;
            }).ToList();
        }
    }
}
