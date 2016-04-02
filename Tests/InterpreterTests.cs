using DynamicInterpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Tests {
    public class InterpreterTests : AssertionHelper {
        [Test, Description("This is just a test to make sure our support 'Handler' objects are working")]
        public void DynamicHandlerTest() {
            var addHandler = Handler.Create("add", args => args.Select(x => x.Apply(y => int.Parse(y.ToString()))).Where(x => x.IsSome).Select(x => x.Value).Sum());
            var result = addHandler.Call(new List<Option<object>>() { "1", "123" });
            Expect(result[0].Value, Is.EqualTo(124));
        }

        [Test]
        public void SimpleDigitTest() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'";
            var codeToRun = "1";
            var addHandler = Handler.Create(Constants.EntryPointSymbolName, args => int.Parse(args.Where(x => x.IsSome).Select(x => x.Value).ToDelimitedString("")));
            var interp = new Interpreter();
            var maybeErr = interp.Setup(grammar, addHandler);
            maybeErr.Apply(x => Expect(false, $"Unexpected error: {x}"));

            var result = interp.Execute(codeToRun);
            Expect(result[0].Value, Is.EqualTo(1), "result was incorrect");
        }

        [Test]
        public void Recursion() {
            var grammar = $"<{Constants.EntryPointSymbolName}> = '1'|'1'<{Constants.EntryPointSymbolName}>";
            var codeToRun = "111";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => int.Parse(args.Where(x => x.IsSome).Select(x => x.Value).ToDelimitedString("")));
            var interp = new Interpreter();
            var maybeErr = interp.Setup(grammar, entryHandler);
            maybeErr.Apply(x => Expect(false, $"Unexpected error: {x}"));

            var result = interp.Execute(codeToRun);
            Expect(result[0].Value, Is.EqualTo(111), "result was incorrect");
        }

        [Test]
        public void MutualRecursion() {
            var grammar = $@"<{Constants.EntryPointSymbolName}> = '1'|'1'<a>
                             <a> = '2'|'2'<{Constants.EntryPointSymbolName}>";
            var codeToRun = "12121";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => args.Where(x => x.IsSome).Select(x => x.Value).ToDelimitedString(""));
            var aHandler = Handler.Create("a", args => args.Where(x => x.IsSome).Select(x => x.Value).ToDelimitedString(""));
            var interp = new Interpreter();
            var maybeErr = interp.Setup(grammar, entryHandler, aHandler);
            maybeErr.Apply(x => Expect(false, $"Unexpected error: {x}"));

            var result = interp.Execute(codeToRun);
            Expect(result[0].Value, Is.EqualTo("12121"), "result was incorrect");
        }

        [Test]
        public void AddTest() {
            var grammar = $@"<{Constants.EntryPointSymbolName}> = <add>
                             <add> = <digit>|<digit>'+'<add>
                             <digit> = '0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9'";

            var codeToRun = "1+2+3+4+5";
            var entryHandler = Handler.Create(Constants.EntryPointSymbolName, args => args.First().Value);
            var digitHandler = Handler.Create("digit", args => int.Parse(args.First().Value.ToString()));
            var addHandler = Handler.Create("add", args => args.Where(x => x.IsSome).Select(x => x.Value.ToString() == "+" ? 0 : (int)x.Value).Sum());
            var interp = new Interpreter();
            var maybeErr = interp.Setup(grammar, entryHandler, digitHandler, addHandler);
            maybeErr.Apply(x => Expect(false, $"Unexpected error: {x}"));

            var result = interp.Execute(codeToRun);
            Expect(result[0].Value, Is.EqualTo(15), "Incorrect result");
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
            var maybeErr = interp.Setup(grammar, entryHandler);
            maybeErr.Apply(x => Expect(false, $"Unexpected error: {x}"));

            var result = interp.Execute(codeToRun);
            Expect(result.Select(x => x.Value).ToDelimitedString(""), Is.EqualTo(codeToRun));
        }
    }
}
