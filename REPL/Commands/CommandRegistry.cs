using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace REPL {
    public class CommandDictionary : IReadOnlyDictionary<string, REPLCommand> {
        static Dictionary<string, REPLCommand> _commands = new Dictionary<string, REPLCommand>();

        static CommandDictionary() {
            var cmdTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(REPLCommand)));
            foreach(var type in cmdTypes) {
                REPLCommand cmd = null;
                try { cmd = (REPLCommand)Activator.CreateInstance(type, _commands); } catch { cmd = (REPLCommand)Activator.CreateInstance(type); }
                _commands[cmd.Name.ToLower()] = cmd;
            }
        }

        public REPLCommand this[string key] { get { return _commands[key]; } }
        public int Count { get { return _commands.Count; } }
        public IEnumerable<string> Keys { get { return _commands.Keys; } }
        public IEnumerable<REPLCommand> Values { get { return _commands.Values; } }
        public bool ContainsKey(string key) => _commands.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, REPLCommand>> GetEnumerator() => _commands.GetEnumerator();
        public bool TryGetValue(string key, out REPLCommand value) => _commands.TryGetValue(key, out value);
        public Option<REPLCommand> TryGetValue(string key) => _commands.TryGetValue(key);
        IEnumerator IEnumerable.GetEnumerator() => _commands.GetEnumerator();
    }
}