using DynamicInterpreter;
using REPL.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public static class ArgsAndSettings {
        static Dictionary<string, Action<string[]>> _options = new Dictionary<string, Action<string[]>>() {
            { "logpath", Values => LogPath = Values.FirstOrDefault() }
        };

        static ArgsAndSettings() {
            SetDefaults();
            LoadSettings();
            LoadArgs();
        }

        public static string LogPath { get; private set; }

        public static void SaveSettings() {
            Settings.Default.Save();
        }

        private static void SetDefaults() {
            LogPath = "";
        }

        private static void LoadSettings() {
            if (Settings.Default.UpdateRequired) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateRequired = false;
            }
        }

        private static void LoadArgs() {
            var argNameHandler = new CombineToStringSymbolHandler("argname");
            var ignoreUpToFirstArgHandler = new IgnoreSymbolHandler("allchars_no_dash");
            var argNameValueSeperatorHandler = new IgnoreSymbolHandler("argname_value_separator");
            var whitespaceHandler = new IgnoreSymbolHandler("allwhitespace");
            var argValueHandler = new CombineToStringSymbolHandler("argvalue");
            var entryHandler = new GenericSymbolHandler(DynamicInterpreter.Constants.EntryPointSymbolName, x => x);

            var argHandler = new GenericSymbolHandler("arg", x => {
                _options.TryGetValue(x[1].Value.ToString()).Apply(y => y(x.Skip(2).Select(z => z.Value.ToString().Trim('\"')).ToArray()));
                return new List<Option<object>>();
            });

            var interp = new Interpreter();
            interp.Setup(Resources.CommandlineGrammar, entryHandler, argHandler, argNameHandler, argNameValueSeperatorHandler, argValueHandler, whitespaceHandler, ignoreUpToFirstArgHandler);
            interp.Execute(Environment.CommandLine);
        }
    }
}
