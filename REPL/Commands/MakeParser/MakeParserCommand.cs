using DynamicInterpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace REPL.MakeParser {
    public class MakeParserCommand : REPLCommand {
        public override string Name { get; } = "MakeParser";
        public override string CommandInfo { get; } = "This command will generate a parser for the specified language.";
        public override string Usage { get; } = "Usage: MakeParser {grammar file name} {destination directory} {parser name}";
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

            ParserCodeGenerator.ParserName = args[2];
            var results = DescriptionLanguageParser.Execute(File.ReadAllText(args[0]));
            if (results.Item2.Count > 0) {
                BetterConsole.WriteOnNextLine($"Failure to load language: {string.Join("\n\n", results.Item2.Select(y => TreePrinter.ToString(y, z => z.SubErrors)))}", ConsoleColor.Red);
            }

            var fileContents = results.Item1.Cast<string>().ToArray();
            File.WriteAllText(Path.Combine(args[1], $"{args[2]}Parser_NoEdit.cs"), fileContents[0]);
            File.WriteAllText(Path.Combine(args[1], $"{args[2]}Parser_Edit.cs"), fileContents[1]);
            File.WriteAllText(Path.Combine(args[1], "InterpreterSupport_NoEdit.cs"), fileContents[2]);
        }
    }
}
