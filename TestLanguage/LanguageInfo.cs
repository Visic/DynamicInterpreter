using REPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLanguage.Properties;

namespace TestLanguage {
    public class LanguageInfo : ILanguageInfo {
        public string Grammar { get; } = Resources.Grammar;
        public string Name { get; } = "Test Language";
    }
}
