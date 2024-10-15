using GOLD;
using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using ScenarioModel.Parsers;
using System.Text;

namespace ScenarioModel.Expressions.Interpreter;

public partial class ExpressionInterpreter
{
    private const string COMPILED_GRAMMAR_EMBEDDED_RESSOURCE = "ScenarioModel.Expressions.Grammar.ExpressionGrammar.egt";

    private Parser _parser { get; init; }

    public ExpressionInterpreter()
    {
        _parser = GoldEngineParserFactory.BuildParser(COMPILED_GRAMMAR_EMBEDDED_RESSOURCE);
    }

    public ExpressionParserResult Parse(string text)
    {
        ExpressionParserResult result = new();

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
    private bool ProcessResponse(ExpressionParserResult result, string text, ParseMessage response)
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

    private static object? Interpret(ExpressionParserResult result, Reduction r)
    {
        ExpressionProductionIndex productionIndex = (ExpressionProductionIndex)r.Parent.TableIndex();
        switch ((ExpressionProductionIndex)r.Parent.TableIndex())
        {
            case ExpressionProductionIndex.Nl_Newline:
                // <nl> ::= NewLine <nl>
                break;

            case ExpressionProductionIndex.Nl_Newline2:
                // <nl> ::= NewLine
                break;

            case ExpressionProductionIndex.Nlo_Newline:
                // <nlo> ::= NewLine <nlo>
                break;

            case ExpressionProductionIndex.Nlo:
                // <nlo> ::= 
                break;

            case ExpressionProductionIndex.String_Identifier:
                // <String> ::= Identifier
                break;

            case ExpressionProductionIndex.String_Stringliteral:
                // <String> ::= StringLiteral
                break;

            case ExpressionProductionIndex.Program:
                // <Program> ::= <Exp> <nlo>
                break;

            case ExpressionProductionIndex.Exp:
                // <Exp> ::= <AndOr Exp>
                break;

            case ExpressionProductionIndex.Andorexp_And:
                // <AndOr Exp> ::= <AndOr Exp> AND <Is Exp>
                break;

            case ExpressionProductionIndex.Andorexp_Or:
                // <AndOr Exp> ::= <AndOr Exp> OR <Is Exp>
                break;

            case ExpressionProductionIndex.Andorexp:
                // <AndOr Exp> ::= <Is Exp>
                break;

            case ExpressionProductionIndex.Isexp_Eqeq:
                // <Is Exp> ::= <Is Exp> '==' <Value Exp>
                break;

            case ExpressionProductionIndex.Isexp_Ltgt:
                // <Is Exp> ::= <Is Exp> '<>' <Value Exp>
                break;

            case ExpressionProductionIndex.Isexp_Exclameq:
                // <Is Exp> ::= <Is Exp> '!=' <Value Exp>
                break;

            case ExpressionProductionIndex.Isexp:
                // <Is Exp> ::= <Value Exp>
                break;

            case ExpressionProductionIndex.Valueexp:
                // <Value Exp> ::= <Value>
                break;

            case ExpressionProductionIndex.Valueexp2:
                // <Value Exp> ::= <Function>
                break;

            case ExpressionProductionIndex.Valueexp3:
                // <Value Exp> ::= <IsRelated>
                break;

            case ExpressionProductionIndex.Valueexp4:
                // <Value Exp> ::= <IsNotRelated>
                break;

            case ExpressionProductionIndex.Valueexp_Lparen_Rparen:
                // <Value Exp> ::= '(' <Exp> ')'
                break;

            case ExpressionProductionIndex.Function_Lparen_Rparen:
                // <Function> ::= <String> '(' <Args> ')'
                break;

            case ExpressionProductionIndex.Args_Comma:
                // <Args> ::= <Args> ',' <Exp>
                break;

            case ExpressionProductionIndex.Args:
                // <Args> ::= <Exp>
                break;

            case ExpressionProductionIndex.Args2:
                // <Args> ::= 
                break;

            case ExpressionProductionIndex.Isrelated_Minusgt:
                // <IsRelated> ::= <String> '->' <String>
                break;

            case ExpressionProductionIndex.Isrelated_Minusgt_Colon:
                // <IsRelated> ::= <String> '->' <String> ':' <String>
                break;

            case ExpressionProductionIndex.Isnotrelated_Minusexclamgt:
                // <IsNotRelated> ::= <String> '-!>' <String>
                break;

            case ExpressionProductionIndex.Isnotrelated_Minusexclamgt_Colon:
                // <IsNotRelated> ::= <String> '-!>' <String> ':' <String>
                break;

            case ExpressionProductionIndex.Value:
                // <Value> ::= <ValueComposite>
                break;

            case ExpressionProductionIndex.Valuecomposite_Dot:
                // <ValueComposite> ::= <ValueComposite> '.' <String>
                break;

            case ExpressionProductionIndex.Valuecomposite:
                // <ValueComposite> ::= <String>
                break;

            default:
                throw new NotImplementedException("Case not handled : " + r.Parent.TableIndex());
        }

        throw new NotImplementedException("Case did not return value" + r.Parent.TableIndex());
    }

}
