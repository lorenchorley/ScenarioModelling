
namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public enum HumanReadableProductionIndex
{
    @Nl_Newline = 0,                           // <nl> ::= NewLine <nl>
    @Nl_Newline2 = 1,                          // <nl> ::= NewLine
    @Nlo_Newline = 2,                          // <nlo> ::= NewLine <nlo>
    @Nlo = 3,                                  // <nlo> ::= 
    @String_Identifier = 4,                    // <String> ::= Identifier
    @String_Stringliteral = 5,                 // <String> ::= StringLiteral
    @Program = 6,                              // <Program> ::= <Definitions>
    @Definitions = 7,                          // <Definitions> ::= <Definitions> <Definition>
    @Definitions2 = 8,                         // <Definitions> ::= <nlo>
    @Definition = 9,                          // <Definition> ::= <NamedDefinition>
    @Definition2 = 10,                         // <Definition> ::= <UnnamedDefinition>
    @Definition3 = 11,                         // <Definition> ::= <NamedLink>
    @Definition4 = 12,                         // <Definition> ::= <UnnamedLink>
    @Definition5 = 13,                         // <Definition> ::= <Transition>
    @Nameddefinition_Lbrace_Rbrace = 14,       // <NamedDefinition> ::= <String> <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Nameddefinition = 15,                     // <NamedDefinition> ::= <String> <String> <nl>
    @Unnameddefinition_Lbrace_Rbrace = 16,     // <UnnamedDefinition> ::= <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Unnameddefinition = 17,                   // <UnnamedDefinition> ::= <String> <nl>
    @Unnamedlink_Minusgt = 18,                 // <UnnamedLink> ::= <String> '->' <String> <nl>
    @Namedlink_Minusgt_Colon = 19,             // <NamedLink> ::= <String> '->' <String> ':' <String> <nl>
    @Transition_Colon = 20                     // <Transition> ::= <String> ':' <String> <nl>
}
