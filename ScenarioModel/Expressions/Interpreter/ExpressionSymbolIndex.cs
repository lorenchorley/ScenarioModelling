namespace ScenarioModel.Expressions.Interpreter;

public enum ExpressionSymbolIndex
{
    @Eof = 0,                                  // (EOF)
    @Error = 1,                                // (Error)
    @Whitespace = 2,                           // Whitespace
    @Exclameq = 3,                             // '!='
    @Minusexclamgt = 4,                        // '-!>'
    @Lparen = 5,                               // '('
    @Rparen = 6,                               // ')'
    @Comma = 7,                                // ','
    @Dot = 8,                                  // '.'
    @Colon = 9,                                // ':'
    @Ltgt = 10,                                // '<>'
    @Eqeq = 11,                                // '=='
    @Minusgt = 12,                             // '->'
    @And = 13,                                 // AND
    @Identifier = 14,                          // Identifier
    @Newline = 15,                             // NewLine
    @Numberliteral = 16,                       // NumberLiteral
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

