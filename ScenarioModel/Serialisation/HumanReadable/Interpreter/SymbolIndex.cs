namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public enum SymbolIndex
{
    @Eof = 0,                                  // (EOF)
    @Error = 1,                                // (Error)
    @Whitespace = 2,                           // Whitespace
    @Colon = 3,                                // ':'
    @Lbrace = 4,                               // '{'
    @Rbrace = 5,                               // '}'
    @Minusgt = 6,                              // '->'
    @Identifier = 7,                           // Identifier
    @Numberliteral = 8,                        // NumberLiteral
    @Stringliteral = 9,                        // StringLiteral
    @Definition = 10,                          // <Definition>
    @Definitions = 11,                         // <Definitions>
    @Id = 12,                                  // <ID>
    @Nameddefinition = 13,                     // <NamedDefinition>
    @Namedlink = 14,                           // <NamedLink>
    @Program = 15,                             // <Program>
    @String = 16,                              // <String>
    @Unnameddefinition = 17,                   // <UnnamedDefinition>
    @Unnamedlink = 18                          // <UnnamedLink>
}

