
namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public enum HumanReadableProductionIndex
{
    @Nl_Newline = 0,                           // <nl> ::= NewLine <nl>
    @Nl_Newline2 = 1,                          // <nl> ::= NewLine
    @Nlo_Newline = 2,                          // <nlo> ::= NewLine <nlo>
    @Nlo = 3,                                  // <nlo> ::= 
    @String_Identifier = 4,                    // <String> ::= Identifier
    @String_Stringliteral = 5,                 // <String> ::= StringLiteral
    @Expressionblock_Expressionblock = 6,      // <ExpressionBlock> ::= ExpressionBlock
    @Program = 7,                              // <Program> ::= <Definitions>
    @Definitions = 8,                          // <Definitions> ::= <Definitions> <Definition>
    @Definitions2 = 9,                         // <Definitions> ::= <nlo>
    @Definition = 10,                          // <Definition> ::= <NamedDefinition>
    @Definition2 = 11,                         // <Definition> ::= <UnnamedDefinition>
    @Definition3 = 12,                         // <Definition> ::= <NamedLink>
    @Definition4 = 13,                         // <Definition> ::= <UnnamedLink>
    @Definition5 = 14,                         // <Definition> ::= <Transition>
    @Definition6 = 15,                         // <Definition> ::= <ExpressionDefinition>
    @Nameddefinition_Lbrace_Rbrace = 16,       // <NamedDefinition> ::= <String> <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Nameddefinition = 17,                     // <NamedDefinition> ::= <String> <String> <nl>
    @Unnameddefinition_Lbrace_Rbrace = 18,     // <UnnamedDefinition> ::= <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Unnameddefinition = 19,                   // <UnnamedDefinition> ::= <String> <nl>
    @Unnamedlink_Minusgt = 20,                 // <UnnamedLink> ::= <String> '->' <String> <nl>
    @Namedlink_Minusgt_Colon = 21,             // <NamedLink> ::= <String> '->' <String> ':' <String> <nl>
    @Expressiondefinition_Lbrace_Rbrace = 22,  // <ExpressionDefinition> ::= <String> <ExpressionBlock> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Expressiondefinition = 23,                // <ExpressionDefinition> ::= <String> <ExpressionBlock> <nl>
    @Expressiondefinition2 = 24,               // <ExpressionDefinition> ::= <ExpressionBlock> <nl>
    @Transition_Colon = 25                     // <Transition> ::= <String> ':' <String> <nl>
}
