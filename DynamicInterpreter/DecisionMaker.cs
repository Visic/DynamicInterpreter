using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

//TODO:: Eventually re-write all of this into a decision graph that can use cycles in the graph to handle recursive definitions instead of recursive data structures (Note:: This is a performance improvement)
namespace DynamicInterpreter {
    //Rule 1: Always take the longest successful match from a definition  (e.g. <a> = '1'|'1'<a>    will greedily consume ones)
    //Rule 2: Multiple definitions can match the same text if the grammar is ambiguous (e.g. <a> = '1'  <b> = '1'    <a> and <b> are both satisfied when a '1' is encountered)

    //TODO:: Need to return on which character (index?) the handler completed. Some of these may not fail until well after one segment of their definition completed.
    //e.g. <a> = 1|11111111111112   will be successful with '1' on 11111111111113, but 11111111111112 will be in progress, and eventually fail, for 13 more characters.

    public enum HandlerState {
        Idle,
        InProgress,
        Complete
    }

    public class DefinitionHandler {
        IReadOnlyList<SequenceHandler> _sequenceHandlers;

        public DefinitionHandler(DefinitionHandler handler) {
            _sequenceHandlers = handler._sequenceHandlers.Select(x => new SequenceHandler(x)).ToArray();
        }

        public DefinitionHandler(IReadOnlyList<Token> def, Dictionary<string, DefinitionHandler> defHandlers) {
            _sequenceHandlers = MakeSequenceHandler(def, defHandlers);
        }

        public void Reset() {
            Methods.For(_sequenceHandlers, x => x.Reset());
        }

        //TODO:: If a handler is in progress when one completes, but then goes to fail some time later, we don't have a way to say we did complete some time ago
        public HandlerState HandleChar(Option<char> ch1, Option<char> ch2) {
            var result = HandlerState.Idle;
            foreach(var ele in _sequenceHandlers) {
                var state = ele.HandleChar(ch1, ch2);

                //Precedence: NotStarted < Completed < InProgress   --unless this is the last character, in which case we ignore InProgress
                if(state != result && (result != HandlerState.InProgress && ch2.IsSome)) result = state;
            }

            if (result == HandlerState.Complete) Reset();
            return result;
        }

        private IReadOnlyList<SequenceHandler> MakeSequenceHandler(IReadOnlyList<Token> def, Dictionary<string, DefinitionHandler> defHandlers) {
            var result = new List<SequenceHandler>();
            var curSeq = new List<Token>();

            foreach (var part in def) {
                if (part.TokenType != Token.Type.Bar) {
                    curSeq.Add(part);
                } else {
                    result.Add(new SequenceHandler(curSeq.ToArray(), defHandlers));
                    curSeq.Clear();
                }
            }
            if (curSeq.Count > 0) result.Add(new SequenceHandler(curSeq.ToArray(), defHandlers));
            return result;
        }
    }

    public class SequenceHandler {
        IReadOnlyList<Token> _definition;
        Dictionary<string, DefinitionHandler> _defHandlers;
        List<Union<char, DefinitionHandler>> _sequence;
        HandlerState _curState = HandlerState.Idle;
        int _curIndex;

        public SequenceHandler(SequenceHandler handler) {
            _defHandlers = handler._defHandlers;
            _sequence = handler._sequence;
            _definition = handler._definition;
        }

        public SequenceHandler(IReadOnlyList<Token> definition, Dictionary<string, DefinitionHandler> defHandlers) {
            _definition = definition;
            _defHandlers = defHandlers;
        }

        public HandlerState HandleChar(Option<char> ch1, Option<char> ch2) {
            if (_sequence == null) InitSequence();
            if (_curState == HandlerState.Complete) return HandlerState.Idle; //already done

            //TODO:: Handle the last character (e.g. ch1.IsSome && ch2.IsNone)
            
            _curState = _sequence[_curIndex].Match<HandlerState>(
                c => ch2.Value == c ? HandlerState.Complete : HandlerState.Idle,
                handler => handler.HandleChar(ch1, ch2)
            );

            switch(_curState) {
                case HandlerState.Idle:
                    if (_curIndex > 0) Reset();
                    break;
                case HandlerState.Complete:
                    if (++_curIndex < _sequence.Count) _curState = HandlerState.InProgress;
                    break;
            }
            return _curState;
        }

        public void Reset() {
            _curIndex = 0;
            _sequence = null;
            _curState = HandlerState.Idle;
        }

        private void InitSequence() {
            _sequence = new List<Union<char, DefinitionHandler>>();
            foreach(var token in _definition) {
                switch(token.TokenType) {
                    case Token.Type.Symbol:
                        _sequence.Add(new DefinitionHandler(_defHandlers[token.Value]));
                        break;
                    case Token.Type.Literal:
                        Methods.For(token.Value, ch => _sequence.Add(ch));
                        break;
                }
            }
        }
    }

    public class DecisionMaker {
        Dictionary<string, DefinitionHandler> _defHandlers = new Dictionary<string, DefinitionHandler>();

        public DecisionMaker(SymbolDefinition[] defs) {
            foreach(var ele in defs) {
                _defHandlers[ele.Name] = new DefinitionHandler(ele.Definition, _defHandlers);
            }
        }

        //return the name of all completed definitions
        public IReadOnlyList<string> HandleChars(Option<char> ch1, Option<char> ch2) {
            return _defHandlers.Where(x => x.Value.HandleChar(ch1, ch2) == HandlerState.Complete).Select(x => x.Key).ToList();
        }
    }
}
