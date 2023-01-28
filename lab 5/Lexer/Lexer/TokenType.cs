using System;
using System.Collections.Generic;

namespace Lexer
{
	public enum TokenType
	{
        IDENTIFIER, // <- string

        KEYWORD, // <- string
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

        NUMBER,// <- char
        // {1234567890}

        SEPARATOR,// <- char
        // _, (, ), ;

        // arithmetic:
        //
        ADD,// <- char
        // +

        SUB,// <- char
        // -

        MUL,// <- char
        // *

        DIV,// <- char
        // /

        ASG,// <- char
        // =

        // comparison:
        //
        EQV,// <- string
        // ==

        NEQV,// <- string
        // !=

        LES,// <- char
        // <

        GRT,// <- char
        // >

        LES_OR_EQV,// <- string
        // <=

        GRT_OR_EQV,// <- string
        // >=

        // logical:
        //
        AND,// <- string
        // &&

        OR,// <- string
        // ||

        // string :
        APOSTROF,// <- char
        // "

        COMMENTARY,// <- string
        // //

        ERROR,
    }
}

