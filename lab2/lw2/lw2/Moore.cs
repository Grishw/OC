using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lw2
{
	public class Moore : Automat
    {
        private static List<string> _outputSignals = new List<string>();

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
                    _outputSignals.RemoveAt(index);
                    foreach (var elem in _signalsActions)
                    {
                        elem.RemoveAt(index);
                    }
                }
            }
        }

        public Moore(StreamReader rs, StreamWriter ws)
        {
            _rs = rs;
            _ws = ws;
        }
    }
}

