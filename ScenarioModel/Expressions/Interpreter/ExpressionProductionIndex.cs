namespace ScenarioModel.Expressions.Interpreter;

public enum ExpressionProductionIndex
{
    @Nl_Newline = 0,                           // <nl> ::= NewLine <nl>
    @Nl_Newline2 = 1,                          // <nl> ::= NewLine
    @Nlo_Newline = 2,                          // <nlo> ::= NewLine <nlo>
    @Nlo = 3,                                  // <nlo> ::= 
    @String_Identifier = 4,                    // <String> ::= Identifier
    @String_Stringliteral = 5,                 // <String> ::= StringLiteral
    @Program = 6,                              // <Program> ::= <Exp> <nlo>
    @Exp = 7,                                  // <Exp> ::= <AndOr Exp>
    @Andorexp_And = 8,                         // <AndOr Exp> ::= <AndOr Exp> AND <Is Exp>
    @Andorexp_Or = 9,                          // <AndOr Exp> ::= <AndOr Exp> OR <Is Exp>
    @Andorexp = 10,                            // <AndOr Exp> ::= <Is Exp>
    @Isexp_Eqeq = 11,                          // <Is Exp> ::= <Is Exp> '==' <Value Exp>
    @Isexp_Ltgt = 12,                          // <Is Exp> ::= <Is Exp> '<>' <Value Exp>
    @Isexp_Exclameq = 13,                      // <Is Exp> ::= <Is Exp> '!=' <Value Exp>
    @Isexp = 14,                               // <Is Exp> ::= <Value Exp>
    @Valueexp = 15,                            // <Value Exp> ::= <Value>
    @Valueexp2 = 16,                           // <Value Exp> ::= <Function>
    @Valueexp3 = 17,                           // <Value Exp> ::= <IsRelated>
    @Valueexp4 = 18,                           // <Value Exp> ::= <IsNotRelated>
    @Valueexp_Lparen_Rparen = 19,              // <Value Exp> ::= '(' <Exp> ')'
    @Function_Lparen_Rparen = 20,              // <Function> ::= <String> '(' <Args> ')'
    @Args_Comma = 21,                          // <Args> ::= <Args> ',' <Exp>
    @Args = 22,                                // <Args> ::= <Exp>
    @Args2 = 23,                               // <Args> ::= 
    @Isrelated_Minusgt = 24,                   // <IsRelated> ::= <String> '->' <String>
    @Isrelated_Minusgt_Colon = 25,             // <IsRelated> ::= <String> '->' <String> ':' <String>
    @Isnotrelated_Minusexclamgt = 26,          // <IsNotRelated> ::= <String> '-!>' <String>
    @Isnotrelated_Minusexclamgt_Colon = 27,    // <IsNotRelated> ::= <String> '-!>' <String> ':' <String>
    @Value = 28,                               // <Value> ::= <ValueComposite>
    @Valuecomposite_Dot = 29,                  // <ValueComposite> ::= <ValueComposite> '.' <String>
    @Valuecomposite = 30                       // <ValueComposite> ::= <String>
}
