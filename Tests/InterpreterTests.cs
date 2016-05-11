using DynamicInterpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using System.Threading;

namespace Tests {
    public class InterpreterTests : AssertionHelper {
        [Test, Description("This is just a test to make sure our support 'Handler' objects are working")]
        public void DynamicHandlerTest() {
            var addHandler = Handler.Create("add", args => args.Select(x => int.Parse(x.ToString())).Sum());
            var result = addHandler.Call(new List<object>() { "1", "123" });
            Expect(result[0], Is.EqualTo(124));
        }

        [Test]
        public void SimpleDigitTest() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'";
            var codeToRun = "1";
            var addHandler = Handler.Create(Constants.EntryPointSymbolName, args => int.Parse(args.ToDelimitedString("")));
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, addHandler);

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo(1), "result was incorrect");
        }

        [Test]
        public void GroupingTest() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '0'('1'|'2''3')";
            var codeToRun = "0231";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo("023"), "result was incorrect");
        }

        [Test, Description("The result should not be a keyword, due to the negative match")]
        public void KeywordTest() {
            var grammar = $@"<char> = 'a'|'b'|'c'
                             <allchars> = <char>|<char><allchars>
                             <keywords> = 'cba'|'abc'
                             <keyword> = <keywords> -<char>
                             <name> = -<keyword><allchars>
                             <{Constants.EntryPointSymbolName}> = <name>|<keyword>";
            var codeToRun = "abca";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler("keyword"));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.Not.EqualTo("abc"), "result was incorrect");
        }

        [Test, Description("Inverting a negative match should result in success with no characters consumed")]
        public void DoubleNegative() {
            var grammar = $"<EntryPoint> = -(-'1')'1'";
            var codeToRun = "11";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo("1"), "result was incorrect");
        }

        [Test]
        public void LiteralWithSlash() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '\\12'";
            var codeToRun = @"\12";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo(@"\12"), "result was incorrect");
        }

        [Test]
        public void LiteralWithQuotes() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '\"123\"'";
            var codeToRun = "\"123\"";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo("\"123\""), "result was incorrect");
        }

        [Test]
        public void PathTest() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = 'C:\\test'";
            var codeToRun = "C:\\test";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo("C:\\test"), "result was incorrect");
        }

        [Test]
        public void EmptyString() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = ''";
            var codeToRun = "1";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo(""), "result was incorrect");
        }

        [Test, Description("Negating the empty string should always fail")]
        public void NotEmptyString() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = -'''1'";
            var codeToRun = "1";
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, new CombineToStringSymbolHandler(Constants.EntryPointSymbolName));

            var result = interp.Execute(codeToRun);
            Expect(result.Item1, Is.Empty, "Negating the empty string should always fail");
        }

        [Test]
        public void Recursion() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '1'|'1'<{Constants.EntryPointSymbolName}>";
            var codeToRun = "111";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => int.Parse(args.ToDelimitedString("")));
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, entryHandler);

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo(111), "result was incorrect");
        }

        [Test]
        public void MutualRecursion() {
            var grammar = $@"<{Constants.EntryPointSymbolName}> = '1'|'1'<a>
                             <a> = '2'|'2'<{Constants.EntryPointSymbolName}>";
            var codeToRun = "12121";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => args.ToDelimitedString(""));
            var aHandler = Handler.Create("a", args => args.ToDelimitedString(""));
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, entryHandler, aHandler);

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo("12121"), "result was incorrect");
        }

        [Test]
        public void AddTest() {
            var grammar = $@"<{Constants.EntryPointSymbolName}> = <add>
                             <add> = <digit>|<digit>'+'<add>
                             <digit> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'";

            var codeToRun = "1+2+3+4+5";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => args.First());
            var digitHandler = Handler.Create("digit", args => int.Parse(args.First().ToString()));
            var addHandler = Handler.Create("add", args => args.Select(x => x.ToString() == "+" ? 0 : (int)x).Sum());
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, entryHandler, digitHandler, addHandler);

            var result = interp.Execute(codeToRun);
            Expect(result.Item1[0], Is.EqualTo(15), "Incorrect result");
        }

        [Test]
        public void SomeHandlersNotDefined() {
            var grammar = $@"<{Constants.EntryPointSymbolName}> = <a>
                             <a> = <digit_or_char>|<digit_or_char><a>
                             <digit_or_char> = <digit>|<char>
                             <digit> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'
                             <char> = 'a'|'b'|'c'";

            var codeToRun = "123abc";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => args);
            var interp = new Interpreter();
            var results = Parser.GenerateParser(grammar);
            Expect(results.Item2, Is.Empty);
            interp.Setup(results.Item1, entryHandler);

            var result = interp.Execute(codeToRun);
            Expect(result.Item1.ToDelimitedString(""), Is.EqualTo(codeToRun));
        }
    }
}
