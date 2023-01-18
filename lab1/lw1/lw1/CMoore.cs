using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace lw1
{
	public class CMoore : Automat
    {
        private static List<string> _outputSignals = new List<string>();

        public void PrintAutomatToFile()
        {
            _ws.WriteLine(FromStringListToString(_outputSignals));
            _ws.WriteLine(FromStringListToString(_states));

            for (int i = 0; i < _inputSignals.Count(); i++)
            {
                _ws.Write(_inputSignals[i]);
                _ws.Write(FromStringListToString(_signalsActions[i]));
                _ws.Write("\n");
            }
        }

        public CMealy TranslateToMealy()
        {
            List<List<string>> mealySignalsActions = new List<List<string>>();

            for (int j = 0; j < _signalsActions.Count(); j++)
            {
                List <string> element = new List<string>();
                for (int i = 0; i < _signalsActions[j].Count(); i++)
                {
                    int pos = _states.IndexOf(_signalsActions[j][i]);
                    element.Add(_signalsActions[j][i] + "/" + _outputSignals[pos]);
                }
                mealySignalsActions.Add(element);
            }

            CMealy mealy = new CMealy(_rs, _ws, ref _inputSignals, ref _states, ref mealySignalsActions);
            return mealy;
        }

        public override void GetDataFromFile()
        {
            _outputSignals = _rs.ReadLine().Split(';').Skip(1).ToList();
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

        public CMoore(StreamReader rs, StreamWriter ws)
		{
            _rs = rs;
            _ws = ws;
        }

        public CMoore(StreamReader rs, StreamWriter ws,
            ref List<string> outputSignals, ref List<string> inputSignals,
            ref List<string> states, ref List<List<string>> signalsActions)
        {
            _rs = rs;
            _ws = ws;
            _outputSignals = outputSignals;
            _inputSignals = inputSignals;
            _states = states;
            _signalsActions = signalsActions;
        }
    }
}

