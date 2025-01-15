namespace ScenarioModelling.Expressions.Interpreter;

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
    @Andorexp_And = 9,                         // <AndOr Exp> ::= <AndOr Exp> AND <Is Exp>
    @Andorexp_Ampamp = 10,                     // <AndOr Exp> ::= <AndOr Exp> '&&' <Is Exp>
    @Andorexp_Or = 11,                         // <AndOr Exp> ::= <AndOr Exp> OR <Is Exp>
    @Andorexp_Pipepipe = 12,                   // <AndOr Exp> ::= <AndOr Exp> '||' <Is Exp>
    @Andorexp = 13,                            // <AndOr Exp> ::= <Is Exp>
    @Isexp_Eqeq = 14,                          // <Is Exp> ::= <Is Exp> '==' <Value Exp>
    @Isexp_Exclameq = 15,                      // <Is Exp> ::= <Is Exp> '!=' <Value Exp>
    @Isexp = 16,                               // <Is Exp> ::= <Value Exp>
    @Valueexp = 17,                            // <Value Exp> ::= <Value>
    @Valueexp2 = 18,                           // <Value Exp> ::= <Function>
    @Valueexp3 = 19,                           // <Value Exp> ::= <IsRelated>
    @Valueexp4 = 20,                           // <Value Exp> ::= <IsNotRelated>
    @Valueexp_Lparen_Rparen = 21,              // <Value Exp> ::= '(' <Exp> ')'
    @Function_Lparen_Rparen = 22,              // <Function> ::= <String> '(' <Args> ')'
    @Args_Comma = 23,                          // <Args> ::= <Args> ',' <Exp>
    @Args = 24,                                // <Args> ::= <Exp>
    @Args2 = 25,                               // <Args> ::= 
    @Isrelated_Minusquestiongt = 26,           // <IsRelated> ::= <String> '-?>' <String>
    @Isrelated_Minusquestiongt_Colon = 27,     // <IsRelated> ::= <String> '-?>' <String> ':' <String>
    @Isnotrelated_Minusexclamgt = 28,          // <IsNotRelated> ::= <String> '-!>' <String>
    @Isnotrelated_Minusexclamgt_Colon = 29,    // <IsNotRelated> ::= <String> '-!>' <String> ':' <String>
    @Value = 30,                               // <Value> ::= <CompositeValue>
    @Valuecomposite_Dot = 31,                  // <CompositeValue> ::= <CompositeValue> '.' <String>
    @Valuecomposite = 32                       // <CompositeValue> ::= <String>
}
