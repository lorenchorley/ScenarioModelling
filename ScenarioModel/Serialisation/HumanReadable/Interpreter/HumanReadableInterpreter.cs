using GOLD;
using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using Isagri.Reporting.StimulSoftMigration.Report.Common.GoldEngine;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public partial class HumanReadableInterpreter
{
    private const string COMPILED_GRAMMAR_EMBEDDED_RESSOURCE = "ScenarioModel.Serialisation.HumanReadable.Grammar.HumanReadableGrammar.egt";

    private GOLD.Parser _parser { get; init; }

    public HumanReadableInterpreter()
    {
        _parser = GoldEngineParserFactory.BuildParser(COMPILED_GRAMMAR_EMBEDDED_RESSOURCE);
    }

    public FilterParserResult Parse(string text)
    {
        FilterParserResult result = new();

        _parser.Open(ref text);
        _parser.TrimReductions = false;

        bool continueParsing = true;
        while (continueParsing && !result.HasErrors)
        {
            ParseMessage response = _parser.Parse();
            continueParsing = ProcessResponse(result, text, response);
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="response"></param>
    /// <returns>Si on devrait continuer le parsing</returns>
    private bool ProcessResponse(FilterParserResult result, string filter, ParseMessage response)
    {
        switch (response)
        {
            case ParseMessage.Reduction:
                _parser.CurrentReduction = Interpret(result, (Reduction)_parser.CurrentReduction);
                break;

            case ParseMessage.Accept:
                // On a fini de parser, on récupère le résultat

                result.Tree = (Definitions)_parser.CurrentReduction;

                return false;

            case ParseMessage.LexicalError:
            case ParseMessage.SyntaxError:
            case ParseMessage.InternalError:
            case ParseMessage.NotLoadedError:
            case ParseMessage.GroupError:
                int column = _parser.CurrentPosition().Column;
                int line = _parser.CurrentPosition().Line;

                if (line == 0)
                {
                    // Faire un message d'erreur qui est bien sympa à lire
                    StringBuilder sb = new();
                    sb.AppendLine("");
                    sb.AppendLine($@"{response} autour de ""{GetSubstringAround(filter, column, 5)}"" dans l'expression suivante :");
                    sb.AppendLine(filter);
                    sb.AppendLine(new string(' ', Math.Max(column - 1, 0)) + "^^^");
                    result.Errors.Add(sb.ToString());
                }
                else
                {
                    // Fallback
                    result.Errors.Add("Normalement on ne doit pas avoir un filtre qui s'étend sur plusieurs lignes");
                    result.Errors.Add($"{response} à la ligne {line} et à la colonne {column}");
                }

                return false;
        }

        return true; // Continue
    }

    private static string GetSubstringAround(string source, int index, int length)
    {
        int start = Math.Max(0, index - length);
        int end = Math.Min(source.Length, index + length);

        return source.Substring(start, end - start);
    }

    private static object? Interpret(FilterParserResult result, Reduction r)
    {
        ProductionIndex productionIndex = (ProductionIndex)r.Parent.TableIndex();
        switch ((ProductionIndex)r.Parent.TableIndex())
        {
            case ProductionIndex.Id_Identifier:
                // <ID> ::= Identifier

                return new IDValue()
                {
                    Value = (string)r[0].Data
                };

            case ProductionIndex.String_Identifier:
                // <String> ::= Identifier

                return new StringValue()
                {
                    Value = (string)r[0].Data
                };

            case ProductionIndex.String_Stringliteral:
                // <String> ::= StringLiteral

                return new StringValue()
                {
                    Value = (string)r[0].Data
                };

            case ProductionIndex.Program:
                // <Program> ::= <Definitions>

                return r.PassOn();

            case ProductionIndex.Definitions:
                // <Definitions> ::= <Definitions> <Definition>
                {

                    Definitions definitions = (Definitions)r[0].Data;
                    Definition definition = (Definition)r[1].Data;

                    definitions.Add(definition);
                    return definitions;

                }

            case ProductionIndex.Definitions2:
                // <Definitions> ::= 

                return new Definitions();

            case ProductionIndex.Definition:
                // <Definition> ::= <NamedDefinition>

                return r.PassOn();

            case ProductionIndex.Definition2:
                // <Definition> ::= <UnnamedDefinition>

                return r.PassOn();

            case ProductionIndex.Definition3:
                // <Definition> ::= <NamedLink>

                return r.PassOn();

            case ProductionIndex.Definition4:
                // <Definition> ::= <UnnamedLink>

                return r.PassOn();

            case ProductionIndex.Nameddefinition_Lbrace_Rbrace:
                // <NamedDefinition> ::= <ID> <String> '{' <Definitions> '}'

                return new NamedDefinition()
                {
                    Type = (IDValue)r[0].Data,
                    Name = (StringValue)r[1].Data,
                    Definitions = (Definitions)r[3].Data
                };

            case ProductionIndex.Nameddefinition:
                // <NamedDefinition> ::= <ID> <String>

                return new NamedDefinition()
                {
                    Type = (IDValue)r[0].Data,
                    Name = (StringValue)r[1].Data
                };

            case ProductionIndex.Unnameddefinition_Lbrace_Rbrace:
                // <UnnamedDefinition> ::= <ID> '{' <Definitions> '}'

                return new UnnamedDefinition()
                {
                    Type = (IDValue)r[0].Data,
                    Definitions = (Definitions)r[2].Data
                };

            case ProductionIndex.Unnameddefinition:
                // <UnnamedDefinition> ::= <ID>

                return new NamedDefinition()
                {
                    Type = (IDValue)r[0].Data
                };

            case ProductionIndex.Unnamedlink_Minusgt:
                // <UnnamedLink> ::= <String> '->' <String>
                return new UnnamedLinkDefinition()
                {
                    Source = (StringValue)r[0].Data,
                    Destination = (StringValue)r[2].Data
                };

            case ProductionIndex.Namedlink_Minusgt_Colon:
                // <NamedLink> ::= <String> '->' <String> ':' <String>

                return new NamedLinkDefinition()
                {
                    Source = (StringValue)r[0].Data,
                    Destination = (StringValue)r[2].Data,
                    Name = (StringValue)r[4].Data
                };

        }

        return null;
    }

}
