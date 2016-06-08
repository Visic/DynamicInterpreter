using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using System.Threading;
using DynamicInterpreter;

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
            var grammar = $"<{"EntryPoint"}> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'";
            var codeToRun = "1";
            var addHandler = Handler.Create("EntryPoint", args => int.Parse(args.ToDelimitedString("")));
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { addHandler });
            Expect(result.Item1[0], Is.EqualTo(1), "result was incorrect");
        }

        [Test]
        public void SimpleDigitRangeTest() {
            var grammar = $"<{"EntryPoint"}> = [0-9]";
            var codeToRun = "1";
            var addHandler = Handler.Create("EntryPoint", args => int.Parse(args.ToDelimitedString("")));
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { addHandler });
            Expect(result.Item1[0], Is.EqualTo(1), "result was incorrect");
        }

        [Test]
        public void GroupingTest() {
            var grammar = $"<{"EntryPoint"}> = '0'('1'|'2''3')";
            var codeToRun = "0231";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint")} );
            Expect(result.Item1[0], Is.EqualTo("023"), "result was incorrect");
        }

        [Test, Description("The result should not be a keyword, due to the negative match")]
        public void KeywordTest() {
            var grammar = $@"<char> = 'a'|'b'|'c'
                             <allchars> = <char>|<char><allchars>
                             <keywords> = 'cba'|'abc'
                             <keyword> = <keywords> -<char>
                             <name> = -<keyword><allchars>
                             <{"EntryPoint"}> = <name>|<keyword>";
            var codeToRun = "abca";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("keyword") });
            Expect(result.Item1[0], Is.Not.EqualTo("abc"), "result was incorrect");
        }

        [Test, Description("Inverting a negative match should result in success with no characters consumed")]
        public void DoubleNegative() {
            var grammar = $"<EntryPoint> = -(-'1')'1'";
            var codeToRun = "11";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint") });
            Expect(result.Item1[0], Is.EqualTo("1"), "result was incorrect");
        }

        [Test]
        public void LiteralWithSlash() {
            var grammar = $"<{"EntryPoint"}> = '\\12'";
            var codeToRun = @"\12";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint") });
            Expect(result.Item1[0], Is.EqualTo(@"\12"), "result was incorrect");
        }

        [Test]
        public void LiteralWithQuotes() {
            var grammar = $"<{"EntryPoint"}> = '\"123\"'";
            var codeToRun = "\"123\"";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint") });
            Expect(result.Item1[0], Is.EqualTo("\"123\""), "result was incorrect");
        }

        [Test]
        public void PathTest() {
            var grammar = $"<{"EntryPoint"}> = 'C:\\test'";
            var codeToRun = "C:\\test";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint") });
            Expect(result.Item1[0], Is.EqualTo("C:\\test"), "result was incorrect");
        }

        [Test]
        public void EmptyString() {
            var grammar = $"<{"EntryPoint"}> = ''";
            var codeToRun = "1";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint") });
            Expect(result.Item1[0], Is.EqualTo(""), "result was incorrect");
        }

        [Test, Description("Negating the empty string should always fail")]
        public void NotEmptyString() {
            var grammar = $"<{"EntryPoint"}> = -'''1'";
            var codeToRun = "1";
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { new CombineToStringSymbolHandler("EntryPoint") });
            Expect(result.Item1, Is.Empty, "Negating the empty string should always fail");
        }

        [Test]
        public void Recursion() {
            var grammar = $"<{"EntryPoint"}> = '1'|'1'<{"EntryPoint"}>";
            var codeToRun = "111";
            var entryHandler = Handler.Create("EntryPoint", args => int.Parse(args.ToDelimitedString("")));
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { entryHandler });
            Expect(result.Item1[0], Is.EqualTo(111), "result was incorrect");
        }

        [Test]
        public void MutualRecursion() {
            var grammar = $@"<{"EntryPoint"}> = '1'|'1'<a>
                             <a> = '2'|'2'<{"EntryPoint"}>";
            var codeToRun = "12121";
            var entryHandler = Handler.Create("EntryPoint", args => args.ToDelimitedString(""));
            var aHandler = Handler.Create("a", args => args.ToDelimitedString(""));
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { entryHandler, aHandler });
            Expect(result.Item1[0], Is.EqualTo("12121"), "result was incorrect");
        }

        [Test]
        public void AddTest() {
            var grammar = $@"<{"EntryPoint"}> = <add>
                             <add> = <digit>|<digit>'+'<add>
                             <digit> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'";

            var codeToRun = "1+2+3+4+5";
            var entryHandler = Handler.Create("EntryPoint", args => args.First());
            var digitHandler = Handler.Create("digit", args => int.Parse(args.First().ToString()));
            var addHandler = Handler.Create("add", args => args.Select(x => x.ToString() == "+" ? 0 : (int)x).Sum());
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { entryHandler, digitHandler, addHandler });
            Expect(result.Item1[0], Is.EqualTo(15), "Incorrect result");
        }

        [Test]
        public void SomeHandlersNotDefined() {
            var grammar = $@"<{"EntryPoint"}> = <a>
                             <a> = <digit_or_char>|<digit_or_char><a>
                             <digit_or_char> = <digit>|<char>
                             <digit> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'
                             <char> = 'a'|'b'|'c'";

            var codeToRun = "123abc";
            var entryHandler = Handler.Create("EntryPoint", args => args);
            var results = LoadLanguageInterpreter.MakeTemporaryParser(grammar);
            Expect(results.Item2, Is.Empty);

            var result = results.Item1(codeToRun, new ISymbolHandler[] { entryHandler });
            Expect(result.Item1.ToDelimitedString(""), Is.EqualTo(codeToRun));
        }
    }
}
