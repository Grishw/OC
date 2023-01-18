using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lw2
{
	public class Automat
	{
        protected StreamReader _rs;
        protected StreamWriter _ws;

        protected static List<string> _inputSignals = new List<string>();

        protected static List<string> _states = new List<string>();
        protected static List<List<string>> _signalsActions = new List<List<string>>(); 

        protected string FromStringListToString(List<string> list)
        {
            string str = "";

            foreach (string elem in list)
            {
                str += $";{elem}";
            }

            return str;
        }

        public void PrintAutomatToFile()
        {
            _ws.WriteLine(FromStringListToString(_states));

            for (int i = 0; i < _inputSignals.Count(); i++)
            {
                _ws.Write(_inputSignals[i]);
                _ws.Write(FromStringListToString(_signalsActions[i]));
                _ws.Write("\n");
            }
        }

        protected HashSet<string> GetAccessibleStateSet()
        {
            HashSet<string> accessibleState = new HashSet<string>();
            for (int j = 0; j < _signalsActions.Count(); j++)
            {
                for (int i = 0; i < _signalsActions[j].Count(); i++)
                {
                    string elem = _signalsActions[j][i].Split("/")[0];
                    if (!accessibleState.Contains(elem))
                    {
                        accessibleState.Add(elem);
                        if (accessibleState.Count() == _states.Count())
                        {
                            return new HashSet<string>(_states);
                        }
                    }

                }
            }

            return accessibleState;
        }

        protected HashSet<string> GetInaccessibleStateSet(ref HashSet<string> accessibleState)
        {
            HashSet<string> inaccessibleState = new HashSet<string>();
            foreach (string state in _states)
            {
                if (!accessibleState.Contains(state))
                {
                    inaccessibleState.Add(state);
                }
            }
            return inaccessibleState;
        }

        // Get Dictionary newEquivalentClasses:
        //
        //   first -> string of (oldClass[i]):(outputSignal[0][i])/(outputSignal[1][i])/(outputSignal[2][i])/...
        //   second -> int index of class
        //
        // {a0} "0:1/2" -> 1
        // {a2} "0:1/3" -> 2
        // {a3} "0:1/2" -> 1
        //
        protected Dictionary<string, int> GetEquivalentClasses(
             List<List<string>> signalActionLinkEquivalentClass,
             Dictionary<string, int> oldStateToEquivalentClassLink)
        {
            Dictionary<string, int> equivalentClasses = new Dictionary<string, int>();
            int index = 0;
            for (int i = 0; i < _states.Count(); i++)
            {
                string key = oldStateToEquivalentClassLink[_states[i]] + ":" + signalActionLinkEquivalentClass[0][i];
                for (int j = 1; j < signalActionLinkEquivalentClass.Count(); j++)
                {
                    key += "/" + signalActionLinkEquivalentClass[j][i];
                }

                if (!equivalentClasses.ContainsKey(key))
                {
                    equivalentClasses[key] = index;
                    index++;
                }
            }

            return equivalentClasses;
        }

        // Create State To classes of Equivalent
        //
        //   first -> string of (state name)
        //   second -> int index of class
        //
        // "a0" -> 1
        // "a2" -> 2
        //
        protected Dictionary<string, int> GetStateToEquivalentClassLink(
             Dictionary<string, int> EquivalentClass,
             List<List<string>> signalActionLinkState,
             Dictionary<string, int> oldStateToEquivalentClassLink)
        {
            Dictionary<string, int> stateToEquivalentClassLink = new Dictionary<string, int>();
            for (int i = 0; i < _states.Count(); i++)
            {
                string key = oldStateToEquivalentClassLink[_states[i]] + ":" + signalActionLinkState[0][i];
                for (int j = 1; j < signalActionLinkState.Count(); j++)
                {
                    key += "/" + signalActionLinkState[j][i];
                }

                stateToEquivalentClassLink[_states[i]] = EquivalentClass[key];
            }

            return stateToEquivalentClassLink;
        }

        //Get list of list:
        //
        //   j -> _inputSignal(list)
        //   i -> _state(list)
        // [j][i] -> outputSignal
        //
        // a0 --z1--> 1(a0 --z1--> "a10" -> 1)
        // a0 --z2--> 2(a0 --z2--> "a1"  -> 2)
        //
        protected List<List<string>> GetSignalActionToEquivalentClassLink(
             List<List<int>> signalActionLinkState,
             Dictionary<string, int> oldStateToEquivalentClassLink)
        {
            List<List<string>> signalActionLinkEquivalentClass = new List<List<string>>();

            for (int j = 0; j < signalActionLinkState.Count(); j++)
            {
                List<string> elem = new List<string>();
                for (int i = 0; i < signalActionLinkState[j].Count(); i++)
                {
                    elem.Add(oldStateToEquivalentClassLink[_states[signalActionLinkState[j][i]]].ToString());
                }
                signalActionLinkEquivalentClass.Add(elem);
            }

            return signalActionLinkEquivalentClass;
        }

        protected virtual void RemoveInaccessibleStates()
        {
            
        }

        public virtual void GetDataFromFile()
        {

        }

        public virtual void Minimize()
        {

        }

    }
}

