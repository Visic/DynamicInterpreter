using DynamicInterpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    public class GrammarParserTests : AssertionHelper {
        [Test]
        public void SimpleParse1() {
            var grammar = "<sym1>='1'";
            var result = GrammarParser.Parse(grammar);
            result.Match(
                symbols => {
                    Expect(symbols.Length, Is.EqualTo(1));
                    Expect(symbols[0].Name, Is.EqualTo("sym1"));
                    Expect(symbols[0].Definition.Count, Is.EqualTo(1));
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo("1"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void SimpleParse2() {
            var grammar = "<sym1>=<a>";
            var result = GrammarParser.Parse(grammar);
            result.Match(
                symbols => {
                    Expect(symbols.Length, Is.EqualTo(1));
                    Expect(symbols[0].Name, Is.EqualTo("sym1"));
                    Expect(symbols[0].Definition.Count, Is.EqualTo(1));
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo("a"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void SimpleParse3_Whitespace() {
            var grammar = "\t<sym1>\n= \t<a>\n";
            var result = GrammarParser.Parse(grammar);
            result.Match(
                symbols => {
                    Expect(symbols.Length, Is.EqualTo(1));
                    Expect(symbols[0].Name, Is.EqualTo("sym1"));
                    Expect(symbols[0].Definition.Count, Is.EqualTo(1));
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo("a"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void Parse_MultiPartDefinition() {
            var grammar = "<testSym> = <a> | '1' | <b>'3'";
            var result = GrammarParser.Parse(grammar);
            result.Match(
                symbols => {
                    Expect(symbols.Length, Is.EqualTo(1));
                    Expect(symbols[0].Name, Is.EqualTo("testSym"));
                    Expect(symbols[0].Definition.Count, Is.EqualTo(6));
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo("a"));
                    Expect(symbols[0].Definition[1].Value, Is.EqualTo("|"));
                    Expect(symbols[0].Definition[2].Value, Is.EqualTo("1"));
                    Expect(symbols[0].Definition[3].Value, Is.EqualTo("|"));
                    Expect(symbols[0].Definition[4].Value, Is.EqualTo("b"));
                    Expect(symbols[0].Definition[5].Value, Is.EqualTo("3"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [TestCase("<<a> = '1'", "<a")]
        [TestCase(@"<\>a> = '1'", ">a")]
        [TestCase(@"<\\\>a> = '1'", @"\>a")]
        [TestCase(@"<\stuff\>a> = '1'", @"\stuff>a")]
        [TestCase(@"<\>\>\\\>\\\><\>> = '1'", @">>\>\><>")]
        public void SymbolContainsBrackets(string grammar, string expectedName) {
            var result = GrammarParser.Parse(grammar);
            result.Match(
                symbols => {
                    Expect(symbols.Length, Is.EqualTo(1));
                    Expect(symbols[0].Name, Is.EqualTo(expectedName));
                    Expect(symbols[0].Definition.Count, Is.EqualTo(1));
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo("1"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [TestCase(@"<a> = '\'1'", "'1")]
        [TestCase(@"<a> = '\'1\''", "'1'")]
        [TestCase(@"<a> = '\'1\'\''", "'1''")]
        [TestCase(@"<a> = '\'aw\'1'", "'aw'1")]
        public void LiteralContainsQuotes(string grammar, string expectedLiteral) {
            var result = GrammarParser.Parse(grammar);
            result.Match(
                symbols => {
                    Expect(symbols.Length, Is.EqualTo(1));
                    Expect(symbols[0].Name, Is.EqualTo("a"));
                    Expect(symbols[0].Definition.Count, Is.EqualTo(1));
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo(expectedLiteral));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void LiteralContainsEquals() {
            var result = GrammarParser.Parse(@"
                <b>=<a>|<c>
                <a>='1'|'='");
            result.Match(
                symbols => {
                    Expect(symbols[1].Definition[0].Value, Is.EqualTo("1"));
                    Expect(symbols[1].Definition[2].Value, Is.EqualTo("="));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void LiteralIsEscapeChar() {
            var result = GrammarParser.Parse(@"<b>='\\'");
            result.Match(
                symbols => {
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo(@"\"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void NegativeMatch1() {
            var result = GrammarParser.Parse(@"<b>=-'test'");
            result.Match(
                symbols => {
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo(@"-"));
                    Expect(symbols[0].Definition[1].Value, Is.EqualTo(@"test"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }

        [Test]
        public void SymbolIsEscapeChar() {
            var result = GrammarParser.Parse(@"<b>=<\\>");
            result.Match(
                symbols => {
                    Expect(symbols[0].Definition[0].Value, Is.EqualTo(@"\"));
                },
                err => Expect(false, $"{err}") //this test should never err
            );
        }
    }
}
