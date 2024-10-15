﻿using GOLD;
using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using ScenarioModel.Parsers;
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

    public HumanReadableParserResult Parse(string text)
    {
        HumanReadableParserResult result = new();

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
    private bool ProcessResponse(HumanReadableParserResult result, string text, ParseMessage response)
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

    private static object? Interpret(HumanReadableParserResult result, Reduction r)
    {
        HumanReadableProductionIndex productionIndex = (HumanReadableProductionIndex)r.Parent.TableIndex();
        switch ((HumanReadableProductionIndex)r.Parent.TableIndex())
        {
            case HumanReadableProductionIndex.Nl_Newline:
                // <nl> ::= NewLine <nl>
                return null;

            case HumanReadableProductionIndex.Nl_Newline2:
                // <nl> ::= NewLine
                return null;

            case HumanReadableProductionIndex.Nlo_Newline:
                // <nlo> ::= NewLine <nlo>
                return null;

            case HumanReadableProductionIndex.Nlo:
                // <nlo> ::= 
                return null;

            case HumanReadableProductionIndex.String_Identifier:
                // <String> ::= Identifier

                return new StringValue()
                {
                    Value = ((string)r[0].Data).Trim('"')
                };

            case HumanReadableProductionIndex.String_Stringliteral:
                // <String> ::= StringLiteral

                return new StringValue()
                {
                    Value = ((string)r[0].Data).Trim('"')
                };

            case HumanReadableProductionIndex.Program:
                // <Program> ::= <Definitions>

                return r.PassOn();

            case HumanReadableProductionIndex.Definitions:
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

            case HumanReadableProductionIndex.Definitions2:
                // <Definitions> ::= <nlo>

                return new Definitions();

            case HumanReadableProductionIndex.Definition:
                // <Definition> ::= <NamedDefinition>
                return r.PassOn();

            case HumanReadableProductionIndex.Definition2:
                // <Definition> ::= <UnnamedDefinition>
                return r.PassOn();

            case HumanReadableProductionIndex.Definition3:
                // <Definition> ::= <NamedLink>
                return r.PassOn();

            case HumanReadableProductionIndex.Definition4:
                // <Definition> ::= <UnnamedLink>
                return r.PassOn();

            case HumanReadableProductionIndex.Definition5:
                // <Definition> ::= <Transition>
                return r.PassOn();

            case HumanReadableProductionIndex.Nameddefinition_Lbrace_Rbrace:
                // <NamedDefinition> ::= <String> <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>

                return new NamedDefinition()
                {
                    Type = (StringValue)r[0].Data,
                    Name = (StringValue)r[1].Data,
                    Definitions = (Definitions)r[5].Data
                };

            case HumanReadableProductionIndex.Nameddefinition:
                // <NamedDefinition> ::= <String> <String> <nl>

                return new NamedDefinition()
                {
                    Type = (StringValue)r[0].Data,
                    Name = (StringValue)r[1].Data
                };

            case HumanReadableProductionIndex.Unnameddefinition_Lbrace_Rbrace:
                // <UnnamedDefinition> ::= <String> <nlo> '{' <nlo> <Definitions> <nlo> '}' <nl>

                return new UnnamedDefinition()
                {
                    Type = (StringValue)r[0].Data,
                    Definitions = (Definitions)r[4].Data
                };

            case HumanReadableProductionIndex.Unnameddefinition:
                // <UnnamedDefinition> ::= <String> <nl>

                return new NamedDefinition()
                {
                    Type = (StringValue)r[0].Data
                };

            case HumanReadableProductionIndex.Unnamedlink_Minusgt:
                // <UnnamedLink> ::= <String> '->' <String> <nl>

                return new UnnamedLinkDefinition()
                {
                    Source = (StringValue)r[0].Data,
                    Destination = (StringValue)r[2].Data
                };

            case HumanReadableProductionIndex.Namedlink_Minusgt_Colon:
                // <NamedLink> ::= <String> '->' <String> ':' <String> <nl>

                return new NamedLinkDefinition()
                {
                    Source = (StringValue)r[0].Data,
                    Destination = (StringValue)r[2].Data,
                    Name = (StringValue)r[4].Data
                };

            case HumanReadableProductionIndex.Transition_Colon:
                // <Transition> ::= <String> ':' <String> <nl>

                return new TransitionDefinition()
                {
                    Type = (StringValue)r[0].Data,
                    TransitionName = (StringValue)r[2].Data
                };

            default:
                throw new NotImplementedException("Case not handled : " + r.Parent.TableIndex());
        }

        throw new NotImplementedException("Case did not return value" + r.Parent.TableIndex());
    }

}