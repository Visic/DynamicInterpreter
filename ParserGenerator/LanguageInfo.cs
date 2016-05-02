using REPL;
using ParserGenerator.Properties;

namespace ParserGenerator {
    public class LanguageInfo : ILanguageInfo {
        public string Grammar { get; } = Resources.Grammar;
        public string Name { get; } = "Parser Generator";
    }
}
