using System;
using System.Collections.Generic;
using System.IO;

namespace Lexer
{
    class Program
    {
        

        static void Main(string[] args)
        {
            string inputF = args[0];

            Lexer lex = new Lexer(inputF);

            try
            {
                while(true)
                {
                    inputF = lex.getNextLexem();
                }
            } catch (Exception e)
            {
                lex.EndWork();
                //Console.WriteLine(e);
            }
        }
    }
}

