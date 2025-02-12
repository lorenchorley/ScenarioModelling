namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.Interpreter;

public enum HumanReadableSymbolIndex
{
    @Eof = 0,                                  // (EOF)
    @Error = 1,                                // (Error)
    @Whitespace = 2,                           // Whitespace
    @Colon = 3,                                // ':'
    @Lbrace = 4,                               // '{'
    @Rbrace = 5,                               // '}'
    @Minusgt = 6,                              // '->'
    @Expressionblock = 7,                      // ExpressionBlock
    @Identifier = 8,                           // Identifier
    @Newline = 9,                              // NewLine
    @Stringliteral = 10,                       // StringLiteral
    @Definition = 11,                          // <Definition>
    @Definitions = 12,                         // <Definitions>
    @Expressionblock2 = 13,                    // <ExpressionBlock>
    @Expressiondefinition = 14,                // <ExpressionDefinition>
    @Nameddefinition = 15,                     // <NamedDefinition>
    @Namedlink = 16,                           // <NamedLink>
    @Nl = 17,                                  // <nl>
    @Nlo = 18,                                 // <nlo>
    @Program = 19,                             // <Program>
    @String = 20,                              // <String>
    @Transition = 21,                          // <Transition>
    @Unnameddefinition = 22,                   // <UnnamedDefinition>
    @Unnamedlink = 23                          // <UnnamedLink>
}

