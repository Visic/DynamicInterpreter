using DynamicInterpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    //public class LoadLanguage : REPLCommand {
    //    bool _languagedLoaded = false;
    //    public override string Name { get; } = "Language";
    //    public override string CommandInfo { get; } = "This command will set the current language.";
    //    public override string Usage { get; } = "Usage: Language {file name}";
    //    public override int RequiredArgNum { get; } = 1;

    //    protected override void ExecuteCmd(List<string> args) {
    //        if(_languagedLoaded) {
    //            BetterConsole.WriteOnNextLine("Language already loaded, unloading languages is not currently supported.");
    //            return;
    //        }

    //        if(!File.Exists(args[0])) {
    //            BetterConsole.WriteOnNextLine($"No file named {args[0]}");
    //            return;
    //        }

    //        try {
    //            var assem = Assembly.LoadFile(args[0]);
    //            var handlers = assem.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ISymbolHandler))).Select(x => (ISymbolHandler)Activator.CreateInstance(x)).ToArray();
    //            var languageInfo = (ILanguageInfo)Activator.CreateInstance(assem.GetTypes().First(x => x.GetInterfaces().Contains(typeof(ILanguageInfo))));
    //            Program.LanguageName = languageInfo.Name;
    //            Program._languageInterp = new Interpreter();
    //            Program._languageInterp.Setup(languageInfo.Grammar, handlers);
    //            _languagedLoaded = true;
    //            BetterConsole.WriteOnNextLine($"{languageInfo.Name} loaded.");
    //        } catch {
    //            BetterConsole.WriteOnNextLine($"{args[0]} is not a path to a valid language assembly.");
    //        }
    //    }
    //}

    public class LoadLanguage : REPLCommand {
        bool _languagedLoaded = false;
        public override string Name { get; } = "Language";
        public override string CommandInfo { get; } = "This command will set the current language.";
        public override string Usage { get; } = "Usage: Language {file name}";
        public override int RequiredArgNum { get; } = 1;

        protected override void ExecuteCmd(List<string> args) {
            if(_languagedLoaded) {
                BetterConsole.WriteOnNextLine("Language already loaded, unloading languages is not currently supported.");
                return;
            }

            if(!File.Exists(args[0])) {
                BetterConsole.WriteOnNextLine($"No file named {args[0]}");
                return;
            }

            try {
                var assem = Assembly.LoadFile(args[0]);
                var handlers = assem.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ISymbolHandler))).Select(x => (ISymbolHandler)Activator.CreateInstance(x)).ToArray();
                var languageInfo = (ILanguageInfo)Activator.CreateInstance(assem.GetTypes().First(x => x.GetInterfaces().Contains(typeof(ILanguageInfo))));
                Program.LanguageName = languageInfo.Name;

                var maybeParser = Parser.GenerateParser(languageInfo.Grammar).Match(
                    x => x,
                    x => BetterConsole.WriteOnNextLine($"Failure to load language: {x}")
                );
                if(maybeParser.IsNone) return;

                Program._languageInterp = new Interpreter();
                Program._languageInterp.Setup(maybeParser.Value, handlers);
                _languagedLoaded = true;
                BetterConsole.WriteOnNextLine($"{languageInfo.Name} loaded.");
            } catch {
                BetterConsole.WriteOnNextLine($"{args[0]} is not a path to a valid language assembly.");
            }
        }
    }
}
