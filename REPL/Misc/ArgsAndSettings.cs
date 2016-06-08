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
        public static Dictionary<string, Action<string[]>> _options = new Dictionary<string, Action<string[]>>() {
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
            CommandlineInterpreter.Execute(Environment.CommandLine);
        }
    }
}
