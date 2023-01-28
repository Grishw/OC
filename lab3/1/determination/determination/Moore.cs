using System;
using System.Collections.Generic;
using System.Linq;

namespace determination
{
    public class Moore
    {
        private const string LEFT_GRAMMAR_TYPE = "left";
        private const string RIGHT_GRAMMAR_TYPE = "right";
        private const string EMPTY_START_STATE = "H";
        private const string EMPTY_FINISH_STATE = "F";
        private const string FINISH_OUTPUT_SIGNAL = "F";
        private const string NO_OUTPUT_SIGNAL = "";
        private const string STATE_DIVIDERS = "/";
        private const string NEW_STATE_BASE_NAME = "S";

        private string _direction = "";

        public List<string> _outputSignals = new List<string>();
        public List<string> _states = new List<string>();
        public List<string> _inputSignals = new List<string>();

        // _signalsActions:
        //
        // i - _state[i]
        // j - _inputSignal[j]
        // [i,j] - in _state[i] get _inputSignal[j] go to _state[_signalsActions[i,j]]
        //
        public List<List<string>> _signalsActions = new List<List<string>>();


        //------------------------------------------------ clear determined automat
        protected HashSet<string> GetAccessibleStateSet( int stateIndex)
        {
            HashSet<string> accessibleState = new HashSet<string>();
            List<string> accessibleStateList = new List<string>();
            int accessibleStateIndex = 0;

            accessibleState.Add(_states[stateIndex]);
            accessibleStateList.Add(_states[stateIndex]);

            while (true)
            {
                for (int j = 0; j < _inputSignals.Count(); j++)
                {
                    string elem = _signalsActions[j][stateIndex];
                    if (elem != "")
                    {
                        if (accessibleStateList.IndexOf(elem) == -1)
                        {
                            accessibleState.Add(elem);
                            accessibleStateList.Add(elem);
                            if (accessibleState.Count() == _states.Count())
                            {
                                return new HashSet<string>(_states);
                            }
                        }
                    }
                }
                accessibleStateIndex += 1;
                if (accessibleStateIndex == accessibleStateList.Count())
                {
                    return accessibleState;
                }
                stateIndex = _states.IndexOf(accessibleStateList[accessibleStateIndex]);
            }
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

        protected void RemoveInaccessibleStates(int startIndex)
        {
            HashSet<string> accessibleState = GetAccessibleStateSet(startIndex );
            if (accessibleState.Count() != _states.Count())
            {
                HashSet<string> inaccessibleState = GetInaccessibleStateSet(ref accessibleState);
                foreach (string state in inaccessibleState)
                {
                    int index = _states.IndexOf(state);
                    _states.RemoveAt(index);
                    _outputSignals.RemoveAt(index);
                    foreach (var elem in _signalsActions)
                    {
                        elem.RemoveAt(index);
                    }
                }
            }
        }
        //------------------------------------------------


        public Moore(string direction, List<string> info = null)
        {
            _direction = direction;
            switch (_direction)
            {
                case LEFT_GRAMMAR_TYPE:
                    SetDataByLeftGramma(info);
                    break;
                case RIGHT_GRAMMAR_TYPE:
                    SetDataByRightGramma(info);
                    break;
                default:
                    throw new ArgumentException("Unavailable grammar type");
            }
        }

        // set data if gramma type is right
        private void SetDataByRightGramma(List<string> data = null)
        {
            _states.Add(EMPTY_FINISH_STATE);
            _outputSignals.Add(FINISH_OUTPUT_SIGNAL);

            //get all states from left side of gramma
            foreach (string grammarRule in data)
            {
                string state = grammarRule.Split(" -> ")[0];
                _states.Add(state);
                _outputSignals.Add("");
            }

            //
            foreach (string grammarRule in data)
            {
                string currentState = grammarRule.Split(" -> ")[0];
                List<string> statesToWhichGo = grammarRule.Split(" -> ")[1].Split(" | ").ToList();
                int indexOfCurrentState = _states.IndexOf(currentState);

                //
                foreach (string stateToWhichGo in statesToWhichGo)
                {
                    bool isInputSymbol = true;
                    string state = "";
                    string inputSymbol = "";

                    //
                    foreach (char ch in stateToWhichGo)
                    {
                        //Pars input state
                        //
                        //alphabit - state
                        //numeric - inputSymbol
                        //
                        if (isInputSymbol && !_states.Contains(ch.ToString()))
                        {
                            inputSymbol += ch;
                        }
                        else
                        {
                            isInputSymbol = false;
                            state += ch;
                        }
                    }

                    if (state == "")
                    {
                        state = EMPTY_FINISH_STATE;
                        _outputSignals[indexOfCurrentState] = NO_OUTPUT_SIGNAL;
                    }

                    // fill set of input signals
                    if (!_inputSignals.Contains(inputSymbol))
                    {
                        _inputSignals.Add(inputSymbol);
                    }

                    // add new state to start authomat
                    // if not some action for this input signal fased befor
                    //
                    int indexOfInputSymbol = _inputSignals.IndexOf(inputSymbol);
                    if (indexOfInputSymbol >= _signalsActions.Count)
                    {
                        List<string> newList = new List<string>();

                        foreach (string st in _states)
                        {
                            newList.Add("");
                        }

                        _signalsActions.Add(newList);
                    }

                    // fill start _signalAction
                    //
                    if (_signalsActions[indexOfInputSymbol][indexOfCurrentState] == "")
                    {
                        _signalsActions[indexOfInputSymbol][indexOfCurrentState] = state; // set
                    }
                    else
                    {
                        _signalsActions[indexOfInputSymbol][indexOfCurrentState] += STATE_DIVIDERS + state; // add
                    }

                }
            }
        }


        // set data if gramma type is left
        private void SetDataByLeftGramma(List<string> data = null)
        {
            _states.Add(EMPTY_START_STATE);
            _outputSignals.Add(NO_OUTPUT_SIGNAL);

            //get all states from left side of gramma
            foreach (string grammarRule in data)
            {
                string state = grammarRule.Split(" -> ")[0];
                _states.Add(state);
                _outputSignals.Add(NO_OUTPUT_SIGNAL);
            }

            //
            foreach (string grammarRule in data)
            {
                string currentState = grammarRule.Split(" -> ")[0];
                List<string> statesFromWhichCome = grammarRule.Split(" -> ")[1].Split(" | ").ToList();
                int indexOfCurrentState = _states.IndexOf(currentState);

                //
                foreach (string stateFromWhichCame in statesFromWhichCome)
                {
                    bool isState = true;
                    string state = "";
                    string inputSymbol = "";

                    foreach (char ch in stateFromWhichCame)
                    {
                        //Pars input state
                        //
                        //alphabit - state
                        //numeric - inputSymbol
                        //
                        if (isState && _states.Contains(ch.ToString()))
                        {
                            state += ch;
                        }
                        else
                        {
                            isState = false;
                            inputSymbol += ch;
                        }
                    }

                    // if needed set start state
                    if (state == "")
                    {
                        state = EMPTY_START_STATE;
                        _outputSignals[indexOfCurrentState] = NO_OUTPUT_SIGNAL; 
                    }

                    // fill set of input signals
                    if (!_inputSignals.Contains(inputSymbol))
                    {
                        _inputSignals.Add(inputSymbol);
                    }


                    // add new signalActions to signalsActions authomat
                    // if not some action for this input signal fased befor
                    //
                    int indexOfInputSymbol = _inputSignals.IndexOf(inputSymbol);
                    if (indexOfInputSymbol >= _signalsActions.Count)
                    {
                        List<string> newList = new List<string>();

                        foreach (string st in _states)
                        {
                            newList.Add("");
                        }

                        _signalsActions.Add(newList);
                    }

                    // fill start _signalAction
                    //
                    int indexOfState = _states.IndexOf(state);
                    if (_signalsActions[indexOfInputSymbol][indexOfState] == "")
                    {
                        _signalsActions[indexOfInputSymbol][indexOfState] += currentState; // set
                    }
                    else
                    {
                        _signalsActions[indexOfInputSymbol][indexOfState] += STATE_DIVIDERS + currentState; // add
                    }

                }
            }

            _outputSignals[1] = FINISH_OUTPUT_SIGNAL;

        }


        //determine automat
        public void Determine()
        {
            List<string> determinedStates = _states;
            List<string> determinedOutputSignals = _outputSignals;
            List<string> finishStates = new List<string>();

            //fill stete that end: that throw finish signal
            for (int i = 0; i < _outputSignals.Count; i++)
            {
                if (_outputSignals[i] == FINISH_OUTPUT_SIGNAL)
                {
                    finishStates.Add(determinedStates[i]);
                }
            }

            List<string> newStates = new List<string>(); // list of state that createted in process of determination. shuld be determinated
            List<List<string>> determinedSignalsAction = _signalsActions;

            // 
            do
            {
                newStates = new List<string>(); // clear

                // fill new state that shul be determined
                foreach (List<string> inputSymbolAction in determinedSignalsAction)
                {
                    foreach (string stateGoTo in inputSymbolAction)
                    {
                        if (stateGoTo.Contains(STATE_DIVIDERS))
                        {
                            newStates.Add(stateGoTo);
                        }
                    }
                }

                foreach (string newState in newStates)
                {
                    string determinedState = newState.Replace(STATE_DIVIDERS, ""); // create new determined state
                    determinedState = new string(determinedState.OrderBy(ch => ch).ToArray());

                    // if alredy fill info about new determined state go to next state
                    if (determinedStates.Contains(determinedState))
                    {
                        break;
                    }

                    //add new state and signal for it to all that have
                    determinedStates.Add(determinedState);
                    determinedOutputSignals.Add("");


                    // new determinated state consist of
                    ISet<string> determinedStatesCharHashSet = newState.Split(STATE_DIVIDERS).ToHashSet<string>();
                    for (int i = 0; i < finishStates.Count; i++)
                    {
                        if (determinedStatesCharHashSet.Contains(finishStates[i]))
                        {
                            determinedOutputSignals[determinedOutputSignals.Count - 1] = FINISH_OUTPUT_SIGNAL;
                        }
                    }

                    // add new signals action for new  determined state
                    foreach (List<string> inputSymbolActions in determinedSignalsAction)
                    {
                        inputSymbolActions.Add("");
                    }

                    //  new determined state consist of states
                    List<string> states = newState.Split(STATE_DIVIDERS).ToList();

                    //  for new state fill signal action
                    foreach (string state in states)
                    {
                        int indexOfState = determinedStates.IndexOf(state);
                        foreach (List<string> inputSymbolActions in determinedSignalsAction)
                        {
                            string stateGoTo = inputSymbolActions[indexOfState];

                            //if non action skip
                            if (stateGoTo == "")
                            {
                                continue;
                            }

                            // fill signal actions
                            if (inputSymbolActions[inputSymbolActions.Count - 1] == "")
                            {
                                inputSymbolActions[inputSymbolActions.Count - 1] = stateGoTo; // set
                            }
                            else
                            {
                                inputSymbolActions[inputSymbolActions.Count - 1] += STATE_DIVIDERS + stateGoTo; // add
                            }
                        }
                    }

                    //delete repit state in signalsActions
                    foreach (List<string> inputSymbolActions in determinedSignalsAction)
                    {
                        ISet<char> determinedStateCharHashSet = determinedState.ToHashSet<char>();

                        for (int indexStateGoTo = 0; indexStateGoTo < inputSymbolActions.Count; indexStateGoTo++)
                        {
                            string transitionFunction = inputSymbolActions[indexStateGoTo];

                            if (transitionFunction.Replace(STATE_DIVIDERS, "").ToHashSet<char>().SetEquals(determinedStateCharHashSet))
                            {
                                inputSymbolActions[indexStateGoTo] = determinedState;
                            }
                        }
                    }

                }
            } while (newStates.Count != 0);


            //clear
            _states = determinedStates;
            _signalsActions = determinedSignalsAction;
            _outputSignals = determinedOutputSignals;

            switch (_direction)
            {
                case LEFT_GRAMMAR_TYPE:
                    RemoveInaccessibleStates(0);
                    break;
                case RIGHT_GRAMMAR_TYPE:
                    RemoveInaccessibleStates(1);
                    break;
                default:
                    throw new ArgumentException("Unavailable grammar type");
            }



            //new name for state
            // old(determinedState) -> new
            Dictionary<string, string> newStatesToDeterminedStates = new Dictionary<string, string>();
            for (int i = 0; i < determinedStates.Count; i++)
            {
                newStatesToDeterminedStates.Add(determinedStates[i], NEW_STATE_BASE_NAME + i);
                Console.WriteLine(determinedStates[i] + " -> " + newStatesToDeterminedStates[determinedStates[i]]);
            }



            // new SignalsAction with new state name
            // old(determinedState) -> new
            List<List<string>> newSignalsActions = determinedSignalsAction;
            foreach (List<string> inputSignalActions in newSignalsActions)
            {
                for (int i = 0; i < inputSignalActions.Count; i++)
                {
                    if (inputSignalActions[i] != "")
                    {
                        inputSignalActions[i] = newStatesToDeterminedStates[inputSignalActions[i]];
                    }
                }
            }

            _states = newStatesToDeterminedStates.Values.ToList();
            _signalsActions = newSignalsActions;
            _outputSignals = determinedOutputSignals;

            
            
        }

        public string GetCsvData()
        {
            string csvData = "";

            csvData += ";";
            if (_outputSignals.Count != 0)
            {
                csvData += _outputSignals.Aggregate((total, part) => $"{total};{part}");
            }
            csvData += "\n";

            csvData += ";";
            csvData += _states.Aggregate((total, part) => $"{total};{part}");
            csvData += "\n";

            for (int i = 0; i < _inputSignals.Count; i++)
            {
                csvData += _inputSignals[i] + ";";
                csvData += _signalsActions[i].Aggregate((total, part) => $"{total};{part}");
                csvData += "\n";
            }

            return csvData;
        }


    }
}

