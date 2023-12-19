using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace lw2
{
    public class Mealy : Automat
    {
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


        // Set some param
        //
        private void SetNewStateParam(
            ref Dictionary<string, int> oldStateToNewStateLink,
            ref int newStateCount,
            ref List<string> newStates,
            ref List<string> takenStates,
            Dictionary<string, int> StateToEquivalentClassLink)
        {
            string commonNewStateName = "S";
            List<int> alredyAddToNewStates = new List<int>();

            foreach(KeyValuePair<string, int> entry in StateToEquivalentClassLink)
            {
                if (!alredyAddToNewStates.Contains(entry.Value))
                {
                    alredyAddToNewStates.Add(entry.Value);
                    takenStates.Add(entry.Key);
                    oldStateToNewStateLink[entry.Key] = newStateCount;
                    newStates.Add(commonNewStateName + newStateCount.ToString());
                    newStateCount++;
                }
                else
                {
                    oldStateToNewStateLink[entry.Key] = alredyAddToNewStates.IndexOf(entry.Value);
                }
            }
        }

        // Get list of list SignalActionToStateLink:
        // what: get signalAction go to new state
        //
        //   j -> _inputSignal(list)
        //   i -> _state(list)
        // [j][i] -> index of state in _state to wich go to
        //
        // a0 --z1--> index(new state)
        // a0 --z2--> index(new state)
        //
        private List<List<int>> GetSignalActionToStateLink()
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


        //Get list of list:
        //
        //   j -> _inputSignal(list)
        //   i -> _state(list)
        // [j][i] -> outputSignal
        //
        private List<List<string>> CreateFirstSignalActionToEquivalentClassLink()
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

        // Create first State To classes of Equivalent
        //
        //   first -> string of (state name)
        //   second -> int index of class
        //
        //   a0 -> 0
        //   a2 -> 0
        //
        private Dictionary<string, int> GetFirstStateToEquivalentClassLink(
             List<List<string>> signalActionLinkState)
        {
            Dictionary<string, int> stateToEquivalentClassLink = new Dictionary<string, int>();
            for (int i = 0; i < _states.Count(); i++)
            {
                stateToEquivalentClassLink[_states[i]] = 0;
            }
            return stateToEquivalentClassLink;

        }

        public override void Minimize()
        {
            //flag of end
            bool isMinimizeEnd = false;

            //clear authomat
            RemoveInaccessibleStates();

            // const on run time
            List<List<int>> signalActionToStateLink = GetSignalActionToStateLink();

            // prepare some data
            List<List<string>> firstSignalActionLinkEquivalentClass = CreateFirstSignalActionToEquivalentClassLink();

            Dictionary<string, int> firstStateToEquivalentClassLink = GetFirstStateToEquivalentClassLink(
                 firstSignalActionLinkEquivalentClass);

            // first step
            Dictionary<string, int> oldEquivalentClasses = GetEquivalentClasses(
                 firstSignalActionLinkEquivalentClass,
                 firstStateToEquivalentClassLink);

            Dictionary<string, int> oldStateToEquivalentClassLink = GetStateToEquivalentClassLink(
                 oldEquivalentClasses,
                 firstSignalActionLinkEquivalentClass,
                 firstStateToEquivalentClassLink); 

            // next steps
            do
            {
                List<List<string>> signalActionLinkEquivalentClass = GetSignalActionToEquivalentClassLink(
                 signalActionToStateLink,
                 oldStateToEquivalentClassLink);
                
                Dictionary<string, int> newEquivalentClasses = GetEquivalentClasses(
                     signalActionLinkEquivalentClass,
                     oldStateToEquivalentClassLink);

                Dictionary<string, int> newStateToEquivalentClassLink = GetStateToEquivalentClassLink(
                     newEquivalentClasses,
                     signalActionLinkEquivalentClass,
                     oldStateToEquivalentClassLink); 

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
            Dictionary<string, int> oldStateToNewStateLink = new Dictionary<string, int>();
            int newStateCount = 0;
            List<string> newStates = new List<string>();
            List<string> takenStates = new List<string>();

            SetNewStateParam(
                ref oldStateToNewStateLink, ref newStateCount, ref newStates,
                ref takenStates, oldStateToEquivalentClassLink);

            //2 create new signalsActions by states
            List<List<string>> newSignalsActions = new List<List<string>>();
            for (int j = 0; j < _inputSignals.Count(); j++)
            {
                List<string> elem = new List<string>();
                for(int index = 0; index < newStates.Count(); index++)
                {
                    string oldAction = _signalsActions[j][_states.IndexOf(takenStates[index])];
                    string newAction = newStates[oldStateToNewStateLink[(oldAction.Split("/")[0])]] +
                        "/" + oldAction.Split("/")[1];
                    elem.Add(newAction);
                }
                newSignalsActions.Add(elem);
            }

            //3 _states = states
            _states = newStates;

            //4 _signalsActions = signalsActions
            _signalsActions = newSignalsActions;
        }

        public Mealy(StreamReader rs, StreamWriter ws)
        {
            _rs = rs;
            _ws = ws;
        }
    }
}

