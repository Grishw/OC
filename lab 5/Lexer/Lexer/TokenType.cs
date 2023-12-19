using System;
using System.Collections.Generic;

namespace Lexer
{
	public enum TokenType
	{
        IDENTIFIER,
        KEYWORD,
        //BEGIN
        //END
        //READ
        //WRITE
        //GET
        //CONST
        //LET
        //VAR
        //IF
        //ELSE
        //WHILE
        //THEN
        //DO
        //FOR
        //TRUE
        //FALSE
        //INTEGER
        //STRING
        NUMBER,
        SEPARATOR,
        // arithmetic:
        //
        ADD,
        SUB,
        MUL,
        DIV,
        ASG,
        // comparison:
        //
        EQV,
        NEQV,
        LES,
        GRT,
        LES_OR_EQV,
        GRT_OR_EQV,
        // logical:
        //
        AND,
        OR,

        // string :
        APOSTROF,
        // "
        STRING,
        COMMENTARY,
        // //
    }
}

