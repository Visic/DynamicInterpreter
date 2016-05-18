using REPL;
using ParserGraphing.Properties;

namespace ParserGraphing {
    public class LanguageInfo : ILanguageInfo {
        public string Grammar { get; } = Resources.Grammar;
        public string Name { get; } = "Parser Graphing";
    }
}
