using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace lw2
{
	public class Mealy : Automat
	{
        public override void GetDataFromFile()
        {
            _states = _rs.ReadLine().Split(';').Skip(1).ToList();

            _inputSignals.Clear();
            _signalsActions.Clear();
            while (!_rs.EndOfStream)
            {
                string[] input = _rs.ReadLine().Split(';');
                if (input.Count() == 0)
                {
                    continue;
                }
                _inputSignals.Add(input[0]);
                _signalsActions.Add(input.Skip(1).ToList());
            }
        }

        protected override void RemoveInaccessibleStates()
        {
            HashSet<string> accessibleState = GetAccessibleStateSet();
            if (accessibleState.Count() != _states.Count())
            {
                HashSet<string> inaccessibleState = GetInaccessibleStateSet(ref accessibleState);
                foreach (string state in inaccessibleState)
                {
                    int index = _states.IndexOf(state);
                    _states.RemoveAt(index);
                    foreach (var elem in _signalsActions)
                    {
                        elem.RemoveAt(index);
                    }
                }
            }
        }

        private List<List<int>> GetSignalActionLinkState()
        {
            List<List<int>> signalActionLinkState = new List<List<int>>();
            for (int j = 0; j < _signalsActions.Count(); j++)
            {
                List<int> elem = new List<int>();
                for (int i = 0; i < _signalsActions[j].Count(); i++)
                {
                    elem.Add(_states.IndexOf(_signalsActions[j][i].Split('/')[0]));
                }
                signalActionLinkState.Add(elem);
            }

            return signalActionLinkState;
        }

        private Dictionary<string, int> GetEquivalentClasses(
            ref List<List<string>> signalActionLinkEquivalentClass)
        {
            Dictionary<string, int> equivalentClasses = new Dictionary<string, int>();
            int index = 0;
            for (int i = 0; i < _states.Count(); i++)
            {
                string key = signalActionLinkEquivalentClass[0][i];
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

        private Dictionary<string, int> GetStateToEquivalentClassLink(
            ref Dictionary<string, int> EquivalentClass,
            ref List<List<string>> signalActionLinkState)
        {
            Dictionary<string, int> stateToEquivalentClassLink = new Dictionary<string, int>();
            for (int i = 0; i < _states.Count(); i++)
            {
                string key = signalActionLinkState[0][i];
                for (int j = 1; j < signalActionLinkState.Count(); j++)
                {
                    key += "/" + signalActionLinkState[j][i];
                }

                stateToEquivalentClassLink[_states[i]] = EquivalentClass[key];
            }

            return stateToEquivalentClassLink;
        }

        private List<List<string>> GetSignalActionLinkEquivalentClass(
            ref List<List<int>> signalActionLinkState,
            ref Dictionary<string, int> oldStateToEquivalentClassLink)
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

        private List<List<string>> CreateFirstSignalActionLinkEquivalentClass()
        {
            List<List<string>> firstSignalActionLinkEquivalentClass = new List<List<string>>();

            for (int j = 0; j < _signalsActions.Count(); j++)
            {
                List<string> elem = new List<string>();
                for (int i = 0; i < _signalsActions[j].Count(); i++)
                {
                    elem.Add(_signalsActions[j][i].Split('/')[1]);
                }
                firstSignalActionLinkEquivalentClass.Add(elem);
            }

            return firstSignalActionLinkEquivalentClass;
        }


        private List<int> CreateNewStates(
                ref Dictionary<string, int> newStateToEquivalentClassLink)
        {
            List<int> stateIndex = new List<int>();
            List<int> alredyAddToStatesIndex = new List<int>();

            foreach(KeyValuePair<string, int> entry in newStateToEquivalentClassLink)
            {
                if (!alredyAddToStatesIndex.Contains(entry.Value))
                {
                    alredyAddToStatesIndex.Add(entry.Value);
                    stateIndex.Add(_states.IndexOf(entry.Key));
                }
            }

            return stateIndex;
        }

        public override void Minimize()
        {
            //flag of end
            bool isMinimizeEnd = false;

            //clear authomat
            RemoveInaccessibleStates();

            // const on run time
            List<List<int>> signalActionLinkState = GetSignalActionLinkState(); // a0 --z1--> index(a10); a0 --z2--> index(a1)

            //prepare some data
            List<List<string>> firstSignalActionLinkEquivalentClass = CreateFirstSignalActionLinkEquivalentClass(); // a0 --z1--> 1(old: a10 -> 1); a0 --z2--> 2(old: a1 -> 2)

            //first step
            // GetEquivalentClasses +add lastEquivalentClasses;
            Dictionary<string, int> oldEquivalentClasses = GetEquivalentClasses(
                ref firstSignalActionLinkEquivalentClass); // {a0} 1/2 -> 1; {a2} 1/3 -> 2
            Dictionary<string, int> oldStateToEquivalentClassLink = GetStateToEquivalentClassLink(
                ref oldEquivalentClasses, ref firstSignalActionLinkEquivalentClass); // a0 -> 1; a2 -> 2

            //next steps
            do
            {
                List<List<string>> signalActionLinkEquivalentClass = GetSignalActionLinkEquivalentClass(
                ref signalActionLinkState, ref oldStateToEquivalentClassLink); // a0 --z1--> 1(old: a10 -> 1); a0 --z2--> 2(old: a1 -> 2)
                Dictionary<string, int> newEquivalentClasses = GetEquivalentClasses(
                    ref signalActionLinkEquivalentClass); // {a0} 1/2 -> 1; {a2} 1/3 -> 2
                Dictionary<string, int> newStateToEquivalentClassLink = GetStateToEquivalentClassLink(
                    ref newEquivalentClasses, ref signalActionLinkEquivalentClass); // a0 -> 1; a2 -> 2

                if (Enumerable.SequenceEqual(oldStateToEquivalentClassLink, newStateToEquivalentClassLink))
                {
                    isMinimizeEnd = true;
                }


                oldEquivalentClasses = newEquivalentClasses;
                oldStateToEquivalentClassLink = newStateToEquivalentClassLink;
            }
            while (!isMinimizeEnd);

            //after minimize
            //1 create new states
            List<int> statesIndex = CreateNewStates(ref oldStateToEquivalentClassLink);
            List<string> states = new List<string>();
            foreach(int index in statesIndex)
            {
                states.Add(_states[index]);
            }
            //2 create new signalsActions by states
            List<List<string>> signalsActions = new List<List<string>>();
            for (int j = 0; j < _inputSignals.Count(); j++)
            {
                List<string> elem = new List<string>();
                foreach (int index in statesIndex)
                {
                    elem.Add(_signalsActions[j][index]);
                }
                signalsActions.Add(elem);
            }
            //3 _states = states
            _states = states;
            //4 _signalsActions = signalsActions
            _signalsActions = signalsActions;
        }

        public Mealy(StreamReader rs, StreamWriter ws)
        {
            _rs = rs;
            _ws = ws;
        }
    }
}

