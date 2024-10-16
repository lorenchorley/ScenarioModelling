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
    @Pipepipe = 11,                            // '||'
    @Ltgt = 12,                                // '<>'
    @Eqeq = 13,                                // '=='
    @Minusgt = 14,                             // '->'
    @And = 15,                                 // AND
    @Identifier = 16,                          // Identifier
    @Newline = 17,                             // NewLine
    @Or = 18,                                  // OR
    @Stringliteral = 19,                       // StringLiteral
    @Andorexp = 20,                            // <AndOr Exp>
    @Args = 21,                                // <Args>
    @Exp = 22,                                 // <Exp>
    @Function = 23,                            // <Function>
    @Isexp = 24,                               // <Is Exp>
    @Isnotrelated = 25,                        // <IsNotRelated>
    @Isrelated = 26,                           // <IsRelated>
    @Nl = 27,                                  // <nl>
    @Nlo = 28,                                 // <nlo>
    @Program = 29,                             // <Program>
    @String = 30,                              // <String>
    @Value = 31,                               // <Value>
    @Valueexp = 32,                            // <Value Exp>
    @Valuecomposite = 33                       // <ValueComposite>
}

