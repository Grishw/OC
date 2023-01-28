using System;
using System.Collections.Generic;
using System.IO;

namespace NDToD
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect arguments count");
                return;
            }

            string inputFile = args[0];
            string outputFile = args[1];

            List<string> fileData = new List<string>();
            using (StreamReader rs = new StreamReader(inputFile))
            {
                while (!rs.EndOfStream)
                {
                    fileData.Add(rs.ReadLine());
                }
            }

            Moore automatFromAlphobite = new Moore(fileData);
            automatFromAlphobite.Determine();

            using (StreamWriter ws = new StreamWriter(outputFile))
            {
                ws.Write(automatFromAlphobite.GetCsvData());
            }


        }
    }
}

