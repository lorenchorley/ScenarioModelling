namespace ScenarioModel.Expressions.Interpreter;

public enum ExpressionSymbolIndex
{
    @Eof = 0,                                  // (EOF)
    @Error = 1,                                // (Error)
    @Whitespace = 2,                           // Whitespace
    @Exclameq = 3,                             // '!='
    @Minusexclamgt = 4,                        // '-!>'
    @Ampamp = 5,                               // '&&'
    @Lparen = 6,                               // '('
    @Rparen = 7,                               // ')'
    @Comma = 8,                                // ','
    @Dot = 9,                                  // '.'
    @Colon = 10,                               // ':'
    @Minusquestiongt = 11,                     // '-?>'
    @Pipepipe = 12,                            // '||'
    @Eqeq = 13,                                // '=='
    @And = 14,                                 // AND
    @Identifier = 15,                          // Identifier
    @Newline = 16,                             // NewLine
    @Or = 17,                                  // OR
    @Stringliteral = 18,                       // StringLiteral
    @Andorexp = 19,                            // <AndOr Exp>
    @Args = 20,                                // <Args>
    @Exp = 21,                                 // <Exp>
    @Function = 22,                            // <Function>
    @Isexp = 23,                               // <Is Exp>
    @Isnotrelated = 24,                        // <IsNotRelated>
    @Isrelated = 25,                           // <IsRelated>
    @Nl = 26,                                  // <nl>
    @Nlo = 27,                                 // <nlo>
    @Program = 28,                             // <Program>
    @String = 29,                              // <String>
    @Value = 30,                               // <Value>
    @Valueexp = 31,                            // <Value Exp>
    @Valuecomposite = 32                       // <ValueComposite>
}

