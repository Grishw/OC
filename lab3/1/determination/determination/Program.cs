using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;

namespace determination
{
    class Program
    {
        public const string LEFT_GRAMMAR_TYPE = "left";
        public const string RIGHT_GRAMMAR_TYPE = "right";


        static Moore ProcessData(string mode, List<string> fileData)
        {
            switch (mode)
            {
                case LEFT_GRAMMAR_TYPE:
                    return new Moore(LEFT_GRAMMAR_TYPE, fileData);
                case RIGHT_GRAMMAR_TYPE:
                    return new Moore(RIGHT_GRAMMAR_TYPE, fileData);
                default:
                    throw new ArgumentException("Unavailable conversion type");
            }
        }

        static void Main(string[] args)
        {

            if (args.Length != 3)
            {
                Console.WriteLine("Incorrect arguments count");
                return;
            }

            string mode = args[0];
            string inputFile = args[1];
            string outputFile = args[2];

            List<string> fileData = new List<string>();
            using (StreamReader rs = new StreamReader(inputFile))
            {
                while (!rs.EndOfStream)
                {
                    fileData.Add(rs.ReadLine());
                }
            }

            Moore automatFromAlphobite = ProcessData(mode, fileData);
            automatFromAlphobite.Determine();

            using (StreamWriter ws = new StreamWriter(outputFile))
            {
                ws.Write(automatFromAlphobite.GetCsvData());
            }


        }
    }
}

