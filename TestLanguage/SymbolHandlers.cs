using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicInterpreter;

namespace TestLanguage {
    public class AddHandler : ISymbolHandler {
        public string SymbolName { get; } = "add";

        public List<object> Call(List<object> args) {
            return new List<object>() { int.Parse(args[0].ToString()) + int.Parse(args[2].ToString()) };
        }
    }

    public class IgnoreWhitespaceHandler : IgnoreSymbolHandler {
        public IgnoreWhitespaceHandler() : base("ignore_whitespace") { }
    }

    public class IntegerHandler : CombineToStringSymbolHandler {
        public IntegerHandler() : base("integer") { }
    }

    public class AllCharsNotStarSlashHandler : CombineToStringSymbolHandler {
        public AllCharsNotStarSlashHandler() : base("allchars_notStarSlash") { }

        public override List<object> Call(List<object> args) {
            return base.Call(args);
        }
    }

    public class FuncNameHandler : CombineToStringSymbolHandler {
        public FuncNameHandler() : base("funcname") { }

        public override List<object> Call(List<object> args) {
            return base.Call(args);
        }
    }

    public class KeywordHandler : CombineToStringSymbolHandler {
        public KeywordHandler() : base("keywords") { }

        public override List<object> Call(List<object> args) {
            return base.Call(args);
        }
    }

    public class BlockCommentHandler : ISymbolHandler {
        public string SymbolName { get; } = "blockcomment";

        public List<object> Call(List<object> args) {
            return new List<object>() { args[1] };
        }
    }
}
