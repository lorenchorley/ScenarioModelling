
namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public enum ProductionIndex
{
    @Id_Identifier = 0,                        // <ID> ::= Identifier
    @String_Identifier = 1,                    // <String> ::= Identifier
    @String_Stringliteral = 2,                 // <String> ::= StringLiteral
    @Program = 3,                              // <Program> ::= <Definitions>
    @Definitions = 4,                          // <Definitions> ::= <Definitions> <Definition>
    @Definitions2 = 5,                         // <Definitions> ::= 
    @Definition = 6,                           // <Definition> ::= <NamedDefinition>
    @Definition2 = 7,                          // <Definition> ::= <UnnamedDefinition>
    @Definition3 = 8,                          // <Definition> ::= <NamedLink>
    @Definition4 = 9,                          // <Definition> ::= <UnnamedLink>
    @Nameddefinition_Lbrace_Rbrace = 10,       // <NamedDefinition> ::= <ID> <String> '{' <Definitions> '}'
    @Nameddefinition = 11,                     // <NamedDefinition> ::= <ID> <String>
    @Unnameddefinition_Lbrace_Rbrace = 12,     // <UnnamedDefinition> ::= <ID> '{' <Definitions> '}'
    @Unnameddefinition = 13,                   // <UnnamedDefinition> ::= <ID>
    @Unnamedlink_Minusgt = 14,                 // <UnnamedLink> ::= <String> '->' <String>
    @Namedlink_Minusgt_Colon = 15              // <NamedLink> ::= <String> '->' <String> ':' <String>
}
