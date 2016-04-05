using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL {
    public static class Constants {
        public const string CmdArgPrefix = "-";
        public const string ReplPromptName = "REPL";

        //command names
        public const string HelpCmdName = "Help";
        public const string ListCmdName = "List";

        //Misc
        public const string UnknownCmdFmtStr = "No command named {{{0}}}\nFor a list of commands type {{List}}";
    }
}
