namespace ScenarioModel.Expressions.Interpreter;

public enum ExpressionProductionIndex
{
    @Nl_Newline = 0,                           // <nl> ::= NewLine <nl>
    @Nl_Newline2 = 1,                          // <nl> ::= NewLine
    @Nlo_Newline = 2,                          // <nlo> ::= NewLine <nlo>
    @Nlo = 3,                                  // <nlo> ::= 
    @String_Identifier = 4,                    // <String> ::= Identifier
    @String_Stringliteral = 5,                 // <String> ::= StringLiteral
    @Program = 6,                              // <Program> ::= <nlo> <Exp> <nlo>
    @Program2 = 7,                             // <Program> ::= <nlo>
    @Exp = 8,                                  // <Exp> ::= <AndOr Exp>
    @Andorexp_And = 9,                         // <AndOr Exp> ::= <Is Exp> AND <AndOr Exp>
    @Andorexp_Ampamp = 10,                     // <AndOr Exp> ::= <Is Exp> '&&' <AndOr Exp>
    @Andorexp_Or = 11,                         // <AndOr Exp> ::= <Is Exp> OR <AndOr Exp>
    @Andorexp_Pipepipe = 12,                   // <AndOr Exp> ::= <Is Exp> '||' <AndOr Exp>
    @Andorexp = 13,                            // <AndOr Exp> ::= <Is Exp>
    @Isexp_Eqeq = 14,                          // <Is Exp> ::= <Value Exp> '==' <Is Exp>
    @Isexp_Ltgt = 15,                          // <Is Exp> ::= <Value Exp> '<>' <Is Exp>
    @Isexp_Exclameq = 16,                      // <Is Exp> ::= <Value Exp> '!=' <Is Exp>
    @Isexp = 17,                               // <Is Exp> ::= <Value Exp>
    @Valueexp = 18,                            // <Value Exp> ::= <Value>
    @Valueexp2 = 19,                           // <Value Exp> ::= <Function>
    @Valueexp3 = 20,                           // <Value Exp> ::= <IsRelated>
    @Valueexp4 = 21,                           // <Value Exp> ::= <IsNotRelated>
    @Valueexp_Lparen_Rparen = 22,              // <Value Exp> ::= '(' <Exp> ')'
    @Function_Lparen_Rparen = 23,              // <Function> ::= <String> '(' <Args> ')'
    @Args_Comma = 24,                          // <Args> ::= <Args> ',' <Exp>
    @Args = 25,                                // <Args> ::= <Exp>
    @Args2 = 26,                               // <Args> ::= 
    @Isrelated_Minusgt = 27,                   // <IsRelated> ::= <String> '->' <String>
    @Isrelated_Minusgt_Colon = 28,             // <IsRelated> ::= <String> '->' <String> ':' <String>
    @Isnotrelated_Minusexclamgt = 29,          // <IsNotRelated> ::= <String> '-!>' <String>
    @Isnotrelated_Minusexclamgt_Colon = 30,    // <IsNotRelated> ::= <String> '-!>' <String> ':' <String>
    @Value = 31,                               // <Value> ::= <ValueComposite>
    @Valuecomposite_Dot = 32,                  // <ValueComposite> ::= <ValueComposite> '.' <String>
    @Valuecomposite = 33                       // <ValueComposite> ::= <String>
}
