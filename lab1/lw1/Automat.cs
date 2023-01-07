using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lw1
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

        public virtual void GetDataFromFile()
        {

        }
    }
}

