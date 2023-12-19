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
            Token lexem = new Token();
            Lexer lexer = new Lexer(inputF);

            try
            {
                while(true)
                {
                    lexem = lexer.getNextLexem();
                    Console.WriteLine($"in line {lexem.y} pos {lexem.x} :  {lexem.lexem}       <- {lexem.stringTokenType}");
                }
            } catch (Exception e)
            {
                lexer.EndWork();
                Console.WriteLine(e.Message);
            }
        }
    }
}

