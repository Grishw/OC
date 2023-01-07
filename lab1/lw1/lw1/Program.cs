using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lw1
{
    class Program
    {
        const string MEALY_TO_MOORE = "mealy-to-moore";
        const string MOORE_TO_MEALY = "moore-to-mealy";
        const string ERROR_MODE_MSG = "Unknown mode";
        

        static void MooreToMealy(StreamReader rs, StreamWriter ws)
        {
            CMoore moore = new CMoore(rs, ws);
            moore.GetDataFromFile();

            CMealy mealy = moore.TranslateToMealy();
            mealy.PrintAutomatToFile();
        }

        static void MealyToMoore(StreamReader rs, StreamWriter ws)
        {
            CMealy mealy = new CMealy(rs, ws);
            mealy.GetDataFromFile();

            CMoore moore = mealy.TranslateToMoore();
            moore.PrintAutomatToFile();
        }

        static void Main(string[] args)
        {
            string mode = args[0];
            string inputFile = args[1];
            string outputFile = args[2];

            using (StreamReader rs = new StreamReader(inputFile))
            {
                using (StreamWriter ws= new StreamWriter(outputFile))
                {
                    if (mode.Equals(MEALY_TO_MOORE))
                    {
                        MealyToMoore(rs, ws);
                    }
                    else if (mode.Equals(MOORE_TO_MEALY))
                    {
                        MooreToMealy(rs, ws);
                    }
                    else
                    {
                        Console.WriteLine(ERROR_MODE_MSG);
                    }
                }
            }
        }
    }
}

