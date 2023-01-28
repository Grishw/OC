using System;
using System.Collections.Generic;
using System.Linq;

namespace NDToD
{
	public class Moore
	{
        private const string FINISH_OUTPUT_SIGNAL = "F";
        private const string EMPTY_SYMBOL = "e";
        private const string DEVIDER = ",";

        private List<string> _outputSignals = new List<string>();
        private List<string> _states = new List<string>();
        private List<string> _inputSignals = new List<string>();

        // _signalsActions:
        //
        // i - _inputSignal[j]
        // j - _state[i]
        // [i,j] - in _state[i] get _inputSignal[j] go to _state[_signalsActions[i,j]]
        //
        private List<List<string>> _signalsActions = new List<List<string>>();

        // _eclose:
        //
        // state -> close e-state
        //
        private Dictionary<string, string> _eclose = new Dictionary<string, string>();
        private bool _isHave_e_InputSignal = false;

        // _finishState:
        //
        // list of states that throw end signal
        //
        private List<string> _finishState = new List<string>();


        //fill _eclose
        private void FillEclose()
        {
            
            //inicialize _eclose
            for (int i = 0; i < _states.Count; i++)
            {
                _eclose.Add(_states[i], _states[i]);
            }

            //have some e input signal?
            int e_inputSignalIndex = _inputSignals.IndexOf(EMPTY_SYMBOL);
            if (e_inputSignalIndex == -1)
            {
                return;
            }
            _isHave_e_InputSignal = true;

            //fill
            for (int i = 0; i < _states.Count; i++)
            {
                if (_signalsActions[e_inputSignalIndex][i] == "")
                {
                    continue;
                }

                int stateIndex = i;
                ISet<string> statesSet = new HashSet<string>(); // set of new state: e-close[i] -> statesSet
                Queue<int> statesIndexQueue = new Queue<int>(); // index of state that shuld be cheked for adding to e-close his "chaild"
                statesIndexQueue.Enqueue(stateIndex);

                while (stateIndex != -1)
                {
                    statesSet.Add(_states[stateIndex]);

                    if (_signalsActions[e_inputSignalIndex][stateIndex] != "")
                    {
                        if (_signalsActions[e_inputSignalIndex][stateIndex].Contains(DEVIDER))
                        {
                            List<string> states = _signalsActions[e_inputSignalIndex][stateIndex].Split(DEVIDER).ToList();
                            foreach (string state in states)
                            {
                                int indexOfState = _states.IndexOf(state);
                                statesIndexQueue.Enqueue(indexOfState);
                            }
                        }
                        else // if one stateGoTo
                        {
                            int indexOfState = _states.IndexOf(_signalsActions[e_inputSignalIndex][stateIndex]);
                            statesIndexQueue.Enqueue(indexOfState);
                        }
                    }

                    //get next stateIndex
                    int newStateIndex = stateIndex;
                    while (newStateIndex == stateIndex && statesIndexQueue.Count > 0 && !statesSet.Contains(_signalsActions[e_inputSignalIndex][newStateIndex]))
                    {
                        newStateIndex = statesIndexQueue.Dequeue();
                    };

                    //if next stateIndex == actual -> end
                    if (newStateIndex == stateIndex)
                    {
                        stateIndex = -1;
                    }
                    else
                    {
                        stateIndex = newStateIndex;
                    }
                }

                _eclose[_states[i]] = new string(String.Join(",", statesSet));
            }


            _inputSignals.RemoveAt(e_inputSignalIndex);
            _signalsActions.RemoveAt(e_inputSignalIndex);
        }

        //fill _finishState
        private void FillFinishState()
        {
            for (int i = 0; i < _outputSignals.Count; i++)
            {
                if (_outputSignals[i] == FINISH_OUTPUT_SIGNAL)
                {
                    _finishState.Add(_states[i]);
                }
            }
        }

        //inicilaze authomat
        public Moore(List<string> data = null)
		{
            if (data == null || data.Count == 0)
            {
                return;
            }

            string[] dirtyOutputSignals = data[0].Split(";");
            _outputSignals = new ArraySegment<string>(dirtyOutputSignals, 1, dirtyOutputSignals.Length - 1).ToList();

            string[] dirtyStates = data[1].Split(";");
            _states = new ArraySegment<string>(dirtyStates, 1, dirtyStates.Length - 1).ToList();

            for (int i = 2; i < data.Count; i++)
            {
                string[] values = data[i].Split(";");
                _inputSignals.Add(values[0]);
                _signalsActions.Add(new ArraySegment<string>(values, 1, values.Length - 1).ToList());
            }


            foreach(List<string> inputSignalActions in _signalsActions)
            {
                for (int i = 0; i < inputSignalActions.Count; i++)
                {
                    if (inputSignalActions[i] == "-")
                    {
                        inputSignalActions[i] = "";
                    }
                }
            }

            FillEclose();
            FillFinishState();

        }


        public void Determine()
        {
            //for not to damage start data and be open to modifay in process
            List<string> determinedStates = new List<string>(_states);
            List<string> determinedOutputSignals = new List<string>(_outputSignals);
            List<string> determinedInputSignals = new List<string>(_inputSignals);
            Dictionary<string, string> eclosures = new Dictionary<string, string>(_eclose);
            List<string> finishStates = new List<string>(_finishState);
            List<List<string>> determinedSignalsActions = new List<List<string>>(_signalsActions);

            List<string> newStates = new List<string>();

            do
            {
                newStates = new List<string>();

                //
                //if haves e in input simbols
                //first step
                //
                if (_isHave_e_InputSignal)
                {
                    newStates = new List<string>() { eclosures[_states[0]] };
                    _isHave_e_InputSignal = false;
                }
                // no e in input simbols: or other steps after first
                else
                {
                    ISet<string> newSet = new HashSet<string>(determinedStates.GetRange(eclosures.Count, determinedStates.Count - eclosures.Count));

                    foreach (List<string> inputSignalActions in determinedSignalsActions)
                    {
                        for (int i = eclosures.Count; i < inputSignalActions.Count; i++)
                        {
                            if (!newSet.Contains(new string(inputSignalActions[i].OrderBy(ch => ch).ToArray())) && inputSignalActions[i] != "")
                            {
                                newStates.Add(inputSignalActions[i]);
                            }
                        }
                    }
                }



                foreach (string newState in newStates)
                {
                    string determinedState = newState.Replace(",", "");
                    // Сортировка символов по алфавиту
                    determinedState = new string(determinedState.OrderBy(ch => ch).ToArray());

                    
                    if (determinedStates.Contains(determinedState) && determinedStates.IndexOf(determinedState) >= _states.Count)
                    {
                        // Убираем запятые stateGoTo -1
                        foreach (List<string> inputSignalActions in determinedSignalsActions)
                        {
                            ISet<char> determinedStateCharHashSet = determinedState.ToHashSet<char>();
                            for (int indexOfStateGoTo = 0; indexOfStateGoTo < inputSignalActions.Count; indexOfStateGoTo++)
                            {
                                string stateGoTo = inputSignalActions[indexOfStateGoTo];
                                if (stateGoTo.Replace(",", "").ToHashSet<char>().SetEquals(determinedStateCharHashSet))
                                {
                                    inputSignalActions[indexOfStateGoTo] = determinedState;
                                }
                            }
                        }
                        continue;
                    }



                    determinedOutputSignals.Add("");
                    ISet<string> determinedStatesCharHashSet = newState.Split(",").ToHashSet<string>();
                    for (int i = 0; i < finishStates.Count; i++)
                    {
                        if (determinedStatesCharHashSet.Contains(finishStates[i]))
                        {
                            determinedOutputSignals[determinedOutputSignals.Count - 1] = FINISH_OUTPUT_SIGNAL;
                        }
                    }

                    //create new string signal action for new state
                    List<string> states = newState.Split(",").ToList();
                    foreach (List<string> inputSignalActions in determinedSignalsActions)
                    {
                        inputSignalActions.Add("");
                    }

                    // Смотрим каждое состояние из newStates
                    // Добавляем функции перехода для нового состояния
                    foreach (string state in states)
                    {
                        int indexOfState = determinedStates.IndexOf(state);
                        foreach (List<string> inputSignalActions in determinedSignalsActions)
                        {
                            string stateGoTo = inputSignalActions[indexOfState];
                            if (stateGoTo == "")
                            {
                                continue;
                            }

                            // if haves e-clous for stateGoTo
                            if (eclosures.ContainsKey(stateGoTo))
                            {
                                stateGoTo = eclosures[stateGoTo];
                            }
                            else
                            {
                                List<string> transitionFunctionStates = stateGoTo.Split(",").ToList();
                                stateGoTo = "";
                                for (int i = 0; i < transitionFunctionStates.Count; i++)
                                {
                                    if (stateGoTo == "")
                                    {
                                        stateGoTo += eclosures[transitionFunctionStates[i]];
                                    }
                                    else if (!stateGoTo.Contains(transitionFunctionStates[i]))
                                    {
                                        stateGoTo += "," + eclosures[transitionFunctionStates[i]];
                                    }
                                }
                            }

                            //fill signalActions for new state
                            if (inputSignalActions[inputSignalActions.Count - 1] == "")
                            {
                                inputSignalActions[inputSignalActions.Count - 1] += stateGoTo;
                            }
                            else
                            {
                                if (!inputSignalActions[inputSignalActions.Count - 1].Contains(stateGoTo))
                                {
                                    string[] statetsGoTo = stateGoTo.Split(",");
                                    foreach (string stateTo in statetsGoTo)
                                    {
                                        if (!inputSignalActions[inputSignalActions.Count - 1].Contains(stateTo))
                                        {
                                            inputSignalActions[inputSignalActions.Count - 1] += "," + stateTo;
                                        }
                                    }
                                }
                            }


                        }
                    }
                    determinedStates.Add(determinedState);
                }
            } while (newStates.Count != 0);


            // new name for created in process state
            //
            // created state -> state
            //
            Dictionary<string, string> newStatesToDeterminedStates = new Dictionary<string, string>();
            for (int i = eclosures.Count; i < determinedStates.Count; i++)
            {
                newStatesToDeterminedStates.Add(determinedStates[i], "S" + (i - eclosures.Count));
            }

            List<List<string>> newSignalsActions = new List<List<string>>();
            foreach (List<string> inputSignalActions in determinedSignalsActions)
            {
                newSignalsActions.Add(inputSignalActions.GetRange(eclosures.Count, inputSignalActions.Count - eclosures.Count));
            }

            determinedOutputSignals = determinedOutputSignals.GetRange(eclosures.Count, determinedOutputSignals.Count - eclosures.Count);

            // Обновляем функции переходов, добавляя новые названия для состояний
            foreach (List<string> inputSignalActions in newSignalsActions)
            {
                for (int i = 0; i < inputSignalActions.Count; i++)
                {
                    if (inputSignalActions[i] != "")
                    {
                        inputSignalActions[i] = newStatesToDeterminedStates[new string(inputSignalActions[i].OrderBy(ch => ch).ToArray())];
                    }
                }
            }

            //set new data
            _inputSignals = determinedInputSignals;
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

