using System;
using System.Collections.Generic;
using System.IO;

namespace Lexer
{

    public class Lexer
	{
        public Dictionary<TokenType, string> tokenTypeToString = new Dictionary<TokenType, string>()
        {
            { TokenType.KEYWORD, "KEYWORD"},
            { TokenType.IDENTIFIER, "IDENTIFIER"},
            { TokenType.NUMBER, "NUMBER"},
            { TokenType.SEPARATOR, "SEPARATOR"},

            { TokenType.ADD, "ADD"},
            { TokenType.SUB, "SUB"},
            { TokenType.MUL, "MUL"},
            { TokenType.DIV, "DIV"},
            { TokenType.ASG, "ASG"},

            { TokenType.EQV, "EQV"},
            { TokenType.NEQV, "NEQV"},
            { TokenType.LES, "LES"},
            { TokenType.GRT, "GRT"},
            { TokenType.LES_OR_EQV, "LES_OR_EQV"},
            { TokenType.GRT_OR_EQV, "GRT_OR_EQV"},
            { TokenType.AND, "AND"},
            { TokenType.OR, "OR"},
            { TokenType.STRING, "STRING"},
            { TokenType.COMMENTARY, "COMMENTARY"}

        };

        public Dictionary<string, TokenType> stringToTokenType = new Dictionary<string, TokenType>()
        {
            { "BEGIN", TokenType.KEYWORD},
            { "END", TokenType.KEYWORD},
            { "READ", TokenType.KEYWORD},
            { "WRITE", TokenType.KEYWORD},
            { "GET", TokenType.KEYWORD},
            { "CONST", TokenType.KEYWORD},
            { "LET", TokenType.KEYWORD},
            { "VAR", TokenType.KEYWORD},
            { "IF", TokenType.KEYWORD},
            { "THEN", TokenType.KEYWORD},
            { "ELSE", TokenType.KEYWORD},
            { "WHILE", TokenType.KEYWORD},
            { "DO", TokenType.KEYWORD},
            { "FOR", TokenType.KEYWORD},
            { "TRUE", TokenType.KEYWORD},
            { "FALSE", TokenType.KEYWORD},
            { "INTEGER", TokenType.KEYWORD},
            { "STRING", TokenType.KEYWORD},

            { "==", TokenType.EQV},
            { "!=", TokenType.NEQV},
            { "<=", TokenType.LES_OR_EQV},
            { ">=", TokenType.GRT_OR_EQV},
            { "&&", TokenType.AND},
            { "||", TokenType.OR},

            { "//", TokenType.COMMENTARY}
        };

        public Dictionary<char, TokenType> charToTokenType = new Dictionary<char, TokenType>()
        {
            { ' ', TokenType.SEPARATOR},
            { '(', TokenType.SEPARATOR},
            { ')', TokenType.SEPARATOR},
            { ';', TokenType.SEPARATOR},
            { ':', TokenType.SEPARATOR},
            { ',', TokenType.SEPARATOR},

            { '+', TokenType.ADD},
            { '-', TokenType.SUB},
            { '*', TokenType.MUL},
            { '/', TokenType.DIV},
            { '=', TokenType.ASG},
            { '<', TokenType.LES},
            { '>', TokenType.GRT},

            { '"', TokenType.APOSTROF},

            //some part of oter that can be seporator
            { '!', TokenType.NEQV},
            { '&', TokenType.AND},
            { '|', TokenType.OR},

        };

        public Dictionary<char, TokenType> charToTokenSecondPart = new Dictionary<char, TokenType>()
        {
            { '/', TokenType.COMMENTARY},
            { '=', TokenType.NEQV},
            { '&', TokenType.AND},
            { '|', TokenType.OR},

        };

        private string _inputFile = "";

		private int _lineIndex = 0;
        private int _lineCount = 0;
        private int _lineFromFileSize = -1;

		private string _buffer = "";
		private string _linefromFile = "";
        private StreamReader _rs;


        public Lexer( string inputF)
		{
            _rs = new StreamReader(inputF);
            _inputFile = inputF;
        }

        public Token getNextLexem()
		{
			if (_lineIndex >= (_lineFromFileSize -1))
			{
                GetNexLineFromFile();
            }

			DeleteStartSpases();
			GetConnectedSimbolsBeforSeporator();

            if (_buffer.Length == 1 && charToTokenSecondPart.ContainsKey(_linefromFile[_lineIndex]))
            {
                _buffer += _linefromFile[_lineIndex];
                _lineIndex += 1;

                if(stringToTokenType[_buffer] == TokenType.COMMENTARY)
                {
                    GetLineToTheEnd();
                    return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                        _buffer, tokenTypeToString[TokenType.COMMENTARY]);
                }
                return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                    _buffer, tokenTypeToString[stringToTokenType[_buffer]]);
            }

            if (_buffer.Length == 1 && charToTokenType.ContainsKey(_buffer[0]))
            {
                if (charToTokenType[_buffer[0]] == TokenType.APOSTROF)
                {
                    GetStringToTheEnd();
                    return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                        _buffer, tokenTypeToString[TokenType.STRING]);
                }
                return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                    _buffer, tokenTypeToString[charToTokenType[_buffer[0]]]);
            }

            if (stringToTokenType.ContainsKey(_buffer))
            {
                return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                    _buffer, tokenTypeToString[stringToTokenType[_buffer]]);
            }

            try
            {
                int numVal = Int32.Parse(_buffer);
                return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                    _buffer, tokenTypeToString[TokenType.NUMBER]);

            }
            catch (FormatException e)
            {
                return new Token(_lineCount, _lineIndex - _buffer.Length + 1,
                    _buffer, tokenTypeToString[TokenType.IDENTIFIER]);
            }
		}


        public void EndWork()
        {
            _rs.Close();
        }


        private void GetNexLineFromFile()
		{
                if (_rs.EndOfStream)
                {
                    throw new Exception("No more lexem in file");
                }

                _linefromFile = _rs.ReadLine();
				_lineIndex = 0;
                _lineCount += 1;
                _lineFromFileSize = _linefromFile.Length;
        }

		private void DeleteStartSpases()
		{
			while (_linefromFile[_lineIndex] == ' ' && _lineIndex < _lineFromFileSize)
			{
				_lineIndex += 1;
            }
		}

        private void GetConnectedSimbolsBeforSeporator()
        {
			_buffer = "";

            do
            {
                _buffer += _linefromFile[_lineIndex];
                _lineIndex += 1;
            }
            while ((_lineIndex < _lineFromFileSize)
            && !charToTokenType.ContainsKey(_linefromFile[_lineIndex])
            && !charToTokenType.ContainsKey(_buffer[0]));
        }

        private void GetLineToTheEnd()
        {
            do
            {
                _buffer += _linefromFile[_lineIndex];
                _lineIndex += 1;
            }
            while (_lineIndex < _lineFromFileSize);
        }


        private void GetStringToTheEnd()
        {
            do
            {
                _buffer += _linefromFile[_lineIndex];
                _lineIndex += 1;
            }
            while ((_lineIndex < _lineFromFileSize)
            && !(charToTokenType.ContainsKey(_buffer[_buffer.Length - 1])
            && charToTokenType[_buffer[_buffer.Length - 1]] == TokenType.APOSTROF));
        }
    }
}

