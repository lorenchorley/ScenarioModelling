
namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public enum ProductionIndex
{
    @Nl_Newline = 0,                           // <nl> ::= NewLine <nl>
    @Nl_Newline2 = 1,                          // <nl> ::= NewLine
    @Nlo_Newline = 2,                          // <nlo> ::= NewLine <nlo>
    @Nlo = 3,                                  // <nlo> ::= 
    @Id_Identifier = 4,                        // <ID> ::= Identifier
    @String_Identifier = 5,                    // <String> ::= Identifier
    @String_Stringliteral = 6,                 // <String> ::= StringLiteral
    @Program = 7,                              // <Program> ::= <Definitions>
    @Definitions = 8,                          // <Definitions> ::= <Definitions> <Definition>
    @Definitions2 = 9,                         // <Definitions> ::= <nlo>
    @Definition = 10,                          // <Definition> ::= <NamedDefinition>
    @Definition2 = 11,                         // <Definition> ::= <UnnamedDefinition>
    @Definition3 = 12,                         // <Definition> ::= <NamedLink>
    @Definition4 = 13,                         // <Definition> ::= <UnnamedLink>
    @Nameddefinition_Lbrace_Rbrace = 14,       // <NamedDefinition> ::= <ID> <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Nameddefinition = 15,                     // <NamedDefinition> ::= <ID> <String> <nl>
    @Unnameddefinition_Lbrace_Rbrace = 16,     // <UnnamedDefinition> ::= <ID> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>
    @Unnameddefinition = 17,                   // <UnnamedDefinition> ::= <ID> <nl>
    @Unnamedlink_Minusgt = 18,                 // <UnnamedLink> ::= <String> '->' <String> <nl>
    @Namedlink_Minusgt_Colon = 19              // <NamedLink> ::= <String> '->' <String> ':' <String> <nl>
}
