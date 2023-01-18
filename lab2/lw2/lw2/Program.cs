using System;
using System.IO;

namespace lw2
{
    class Program
    {
        const string MEALY = "mealy";
        const string MOORE = "moore";
        const string ERROR_TYPE = "Unknown type";


        static void MooreMinimize(StreamReader rs, StreamWriter ws)
        {
            Moore moore = new Moore(rs, ws);
            moore.GetDataFromFile();
            moore.Minimize();
            moore.PrintAutomatToFile();
        }

        static void MealyMinimize(StreamReader rs, StreamWriter ws)
        {
            Mealy mealy = new Mealy(rs, ws);
            mealy.GetDataFromFile();
            mealy.Minimize();
            mealy.PrintAutomatToFile();
        }

        static void Main(string[] args)
        {
            string mode = args[0];
            string inputFile = args[1];
            string outputFile = args[2];

            using (StreamReader rs = new StreamReader(inputFile))
            {
                using (StreamWriter ws = new StreamWriter(outputFile))
                {
                    if (mode.Equals(MEALY))
                    {
                        MealyMinimize(rs, ws);
                    }
                    else if (mode.Equals(MOORE))
                    {
                        MooreMinimize(rs, ws);
                    }
                    else
                    {
                        Console.WriteLine(ERROR_TYPE);
                    }
                }
            }
        }
    }
}

