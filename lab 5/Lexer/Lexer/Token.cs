using System;
namespace Lexer
{
	public struct Token
	{
        public string stringTokenType { get; set; }
        public string lexem { get; set; }
        public int y { get; set; }
        public int x { get; set; }

        public Token(int theLinePosition, int inLinePosition, string lex, string tokenType)
        {
            stringTokenType = tokenType;
            lexem = lex;
            y = theLinePosition;
            x = inLinePosition;
        }
    }
}

