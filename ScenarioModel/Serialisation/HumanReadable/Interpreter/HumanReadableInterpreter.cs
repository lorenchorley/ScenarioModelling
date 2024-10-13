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

        text += "\r\n";

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
    private bool ProcessResponse(FilterParserResult result, string text, ParseMessage response)
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
                int columnNumber = _parser.CurrentPosition().Column;
                int lineNumber = _parser.CurrentPosition().Line;
                var lines = text.Replace("\r", "").Split('\n');

                StringBuilder sb = new();

                sb.AppendLine(response.ToString());
                sb.AppendLine("---");
                sb.AppendLine("");

                for (int i = 0; i < lines.Length; i++)
                {
                    if (i != lineNumber)
                    {
                        sb.AppendLine(lines[i]);
                        continue;
                    }

                    sb.AppendLine(lines[i]);
                    sb.AppendLine(new string(' ', Math.Max(columnNumber, 0)) + "^ <- Error here");
                }

                result.Errors.Add(sb.ToString());

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
            case ProductionIndex.Nl_Newline:
                // <nl> ::= NewLine <nl>
                return null;

            case ProductionIndex.Nl_Newline2:
                // <nl> ::= NewLine
                return null;

            case ProductionIndex.Nlo_Newline:
                // <nlo> ::= NewLine <nlo>
                return null;

            case ProductionIndex.Nlo:
                // <nlo> ::= 
                return null;

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
                    Value = ((string)r[0].Data).Trim('"')
                };

            case ProductionIndex.String_Stringliteral:
                // <String> ::= StringLiteral

                return new StringValue()
                {
                    Value = ((string)r[0].Data).Trim('"')
                };

            case ProductionIndex.Program:
                // <Program> ::= <Definitions>

                return r.PassOn();

            case ProductionIndex.Definitions:
                // <Definitions> ::= <Definitions> <Definition>
                {

                    Definitions definitions = (Definitions)r[0].Data;

                    if (r[1].Data != null)
                    {
                        Definition definition = (Definition)r[1].Data;
                        definitions.Add(definition);
                    }

                    return definitions;

                }

            case ProductionIndex.Definitions2:
                // <Definitions> ::= <nlo>

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
                // <NamedDefinition> ::= <ID> <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>

                return new NamedDefinition()
                {
                    Type = (IDValue)r[0].Data,
                    Name = (StringValue)r[1].Data,
                    Definitions = (Definitions)r[5].Data
                };

            case ProductionIndex.Nameddefinition:
                // <NamedDefinition> ::= <ID> <String> <nl>

                return new NamedDefinition()
                {
                    Type = (IDValue)r[0].Data,
                    Name = (StringValue)r[1].Data
                };

            case ProductionIndex.Unnameddefinition_Lbrace_Rbrace:
                // <UnnamedDefinition> ::= <ID> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>

                return new UnnamedDefinition()
                {
                    Type = (IDValue)r[0].Data,
                    Definitions = (Definitions)r[4].Data
                };

            case ProductionIndex.Unnameddefinition:
                // <UnnamedDefinition> ::= <ID> <nl>

                return new NamedDefinition()
                {
                    Type = (IDValue)r[0].Data
                };

            case ProductionIndex.Unnamedlink_Minusgt:
                // <UnnamedLink> ::= <String> '->' <String> <nl>

                return new UnnamedLinkDefinition()
                {
                    Source = (StringValue)r[0].Data,
                    Destination = (StringValue)r[2].Data
                };

            case ProductionIndex.Namedlink_Minusgt_Colon:
                // <NamedLink> ::= <String> '->' <String> ':' <String> <nl>

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
