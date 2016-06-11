using DynamicInterpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace REPL.MakeInterpreter {
    public class MakeInterpreterCommand : REPLCommand {
        public override string Name { get; } = "Create";
        public override string CommandInfo { get; } = "This command will generate an interpreter for the specified language.";
        public override string Usage { get; } = "Usage: Create {grammar file path} {destination directory} {language name}";
        public override int RequiredArgNum { get; } = 3;

        protected override void ExecuteCmd(List<string> args) {
            if (!File.Exists(args[0])) {
                BetterConsole.WriteOnNextLine($"Grammar file does not exist.", ConsoleColor.Red);
                return;
            }

            if(!Directory.Exists(args[1])) {
                BetterConsole.WriteOnNextLine($"Output directory does not exist.", ConsoleColor.Red);
                return;
            }

            InterpreterCodeGenerator.LanguageName = args[2];
            var results = DescriptionLanguageInterpreter.Execute(File.ReadAllText(args[0]));
            if (results.Item2.Count > 0) {
                BetterConsole.WriteOnNextLine($"Failure to load language: {string.Join("\n\n", results.Item2.Select(y => TreePrinter.ToString(y, z => z.SubErrors)))}", ConsoleColor.Red);
            }

            var fileContents = results.Item1.Cast<string>().ToArray();
            File.WriteAllText(Path.Combine(args[1], $"{args[2]}Interpreter_NoEdit.cs"), fileContents[0]);
            File.WriteAllText(Path.Combine(args[1], $"{args[2]}Interpreter_Edit.cs"), fileContents[1]);
            File.WriteAllText(Path.Combine(args[1], "InterpreterSupport_NoEdit.cs"), fileContents[2]);
        }
    }
}
