namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public enum HumanReadableSymbolIndex
{
    @Eof = 0,                                  // (EOF)
    @Error = 1,                                // (Error)
    @Whitespace = 2,                           // Whitespace
    @Colon = 3,                                // ':'
    @Lbrace = 4,                               // '{'
    @Rbrace = 5,                               // '}'
    @Minusgt = 6,                              // '->'
    @Identifier = 7,                           // Identifier
    @Newline = 8,                              // NewLine
    @Numberliteral = 9,                        // NumberLiteral
    @Stringliteral = 10,                       // StringLiteral
    @Definition = 11,                          // <Definition>
    @Definitions = 12,                         // <Definitions>
    @Id = 13,                                  // <ID>
    @Nameddefinition = 14,                     // <NamedDefinition>
    @Namedlink = 15,                           // <NamedLink>
    @Nl = 16,                                  // <nl>
    @Nlo = 17,                                 // <nlo>
    @Program = 18,                             // <Program>
    @String = 19,                              // <String>
    @Transition = 20,                          // <Transition>
    @Unnameddefinition = 21,                   // <UnnamedDefinition>
    @Unnamedlink = 22                          // <UnnamedLink>
}

