using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lw1
{
    public class CMealy : Automat
    {
        const string NEW_POINT_NAME_PART = "S";

        public override void PrintAutomatToFile()
        {
            _ws.WriteLine(FromStringListToString(_states));

            for (int i = 0; i < _inputSignals.Count(); i++)
            {
                _ws.Write(_inputSignals[i]);
                _ws.Write(FromStringListToString(_signalsActions[i]));
                _ws.Write("\n");
            }
        }

        public CMoore TranslateToMoore()
        {
            Dictionary<string, string> StateAndSignal = new Dictionary<string, string>();//old -> new
            int index = 0;
            for (int j = 0; j < _signalsActions.Count(); j++)
            {
                for (int i = 0; i < _signalsActions[j].Count(); i++)
                {
                    if(!StateAndSignal.ContainsKey(_signalsActions[j][i]))
                    {
                        StateAndSignal[$"{_signalsActions[j][i]}"] = $"{NEW_POINT_NAME_PART}{index}";
                        index++;
                    }
                }
            }

            List<string> outputSignals = new List<string>();
            List<string> newstates = new List<string>();
            List<string> oldstates = new List<string>();
            foreach (KeyValuePair<string, string> entry in StateAndSignal)
            {
                newstates.Add(entry.Value);
                outputSignals.Add(entry.Key.Split("/")[1]);
                oldstates.Add(entry.Key.Split("/")[0]);
            }

            index = 0;
            List<List<string>> mooreSignalsActions = new List<List<string>>();
            for (int j = 0; j < _inputSignals.Count(); j++)
            {
                List<string> element = new List<string>();
                for (int i = 0; i < oldstates.Count(); i++)
                {
                    index = _states.IndexOf(oldstates[i]);
                    element.Add(StateAndSignal[_signalsActions[j][index]]);
                }
                mooreSignalsActions.Add(element);
            }

            return new CMoore(_rs, _ws, ref outputSignals, ref _inputSignals,
                ref newstates, ref mooreSignalsActions);
        }

        public override void GetDataFromFile()
        {
            _states = _rs.ReadLine().Split(';').Skip(1).ToList();

            _inputSignals.Clear();
            _signalsActions.Clear();
            while (!_rs.EndOfStream)
            {
                string[] input = _rs.ReadLine().Split(';');
                _inputSignals.Add(input[0]);
                _signalsActions.Add(input.Skip(1).ToList());
            }
        }

        public CMealy(StreamReader rs, StreamWriter ws)
        {
            _rs = rs;
            _ws = ws;
        }

        public CMealy(StreamReader rs, StreamWriter ws,
             ref List<string> inputSignals,
            ref List<string> states, ref List<List<string>> signalsActions)
        {
            _rs = rs;
            _ws = ws;
            _inputSignals = inputSignals;
            _states = states;
            _signalsActions = signalsActions;
        }
    }
}

