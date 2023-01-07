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

