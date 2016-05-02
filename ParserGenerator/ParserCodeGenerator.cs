using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ParserGenerator {
    public static class ParserCodeGenerator {
        public static string Literal(string value) => $"() => Literal(\"{value}\", data, acc)";
        public static string Symbol(string symbolName) => $"_symbols[\"{symbolName}\"](data, acc)";
        public static string Negate(string parserToNegate) => $"Negate(() => {parserToNegate}, data, acc)";
        public static string InOrder(IEnumerable<string> parsers) => $"() => InOrder({string.Join(", ", parsers)}, data, acc)";
        public static string Any(IEnumerable<string> parsers) => $"() => Any({string.Join(", ", parsers)}, data, acc)";
        public static string Assignment(string newParserName, string parser) => $"{{\"{newParserName}\" = {parser}}}";

        //Tuple<State, string> Parse(string data, Result acc)
        public static string EntryPoint(IEnumerable<string> allAssignments) {
            return
$@"public static class Parser {{
    public delegate Tuple<State, string> Parse(string data, Result acc);
    Dictionary<string, Parse> _symbols = new Dictionary<string, Parse>(){{
        {string.Join(",\n\t\t", allAssignments.ToArray())}
    }};

    public static Dictionary<string, Parse> MakeParsers(string definition) {{
        return _symbols;
    }}

    private static Tuple<State, string> Literal(string value, string data, Result acc) {{
        if(data.StartsWith(value)) {{
            acc.Add(value);
            return Tuple.Create(State.Success, data.Remove(0, value.Length));
        }}
        return Tuple.Create(State.Failure, data);
    }}

    private static Tuple<State, string> Negate(Parse parserToNegate, string data, Result acc) {{
        var newAcc = new Result();
        var result = parserToNegate(data, newAcc);
        if(result.Item1 == State.Success) return Tuple.Create(State.Failure, data);
        return Tuple.Create(State.Success, data);
    }}

    private static Tuple<State, string> InOrder(Parse[] parsers, string data, Result acc) {{
        var newAcc = new Result();
        var result = data;
        foreach (var parser in parsers) {{
            var partialResult = parser(result, newAcc);
            if (partialResult.Item1 == State.Failure) return Tuple.Create(State.Failure, data);
            result = partialResult.Item2;
        }}
        acc.AddRange(newAcc);
        return Tuple.Create(State.Success, result);
    }}

    private static Tuple<State, string> Any(Parse[] parsers, string data, Result acc) {{
        var result = parsers.Select(
            x => {{
                var newAcc = new Result();
                return new {{ result = x(data, newAcc), acc = newAcc }};
            }}
        ).Where(x => x.result.Item1 == State.Success).ToArray();

        if(result.Length == 0) return Tuple.Create(State.Failure, data);
        var longestMatch = result.MinBy(x => x.result.Item2.Length);
        acc.AddRange(longestMatch.acc);
        return longestMatch.result;
    }}
}}";
        }
    }
}
