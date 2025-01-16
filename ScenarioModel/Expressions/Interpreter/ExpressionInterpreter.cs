using GOLD;
using ScenarioModelling.Expressions.Grammar;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Parsers;
using System.Text;

namespace ScenarioModelling.Expressions.Interpreter;

public partial class ExpressionInterpreter
{
    private static string COMPILED_GRAMMAR_EMBEDDED_RESSOURCE = $"{ExpressionGrammarLocalisation.Namespace}.ExpressionGrammar.egt";

    private Parser _parser { get; set; }

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

                result.ParsedObject = (Expression)_parser.CurrentReduction;

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
                return null;

            case ExpressionProductionIndex.Nl_Newline2:
                // <nl> ::= NewLine
                return null;

            case ExpressionProductionIndex.Nlo_Newline:
                // <nlo> ::= NewLine <nlo>
                return null;

            case ExpressionProductionIndex.Nlo:
                // <nlo> ::= 
                return null;

            case ExpressionProductionIndex.String_Identifier:
                // <String> ::= Identifier
                return r.PassOn();

            case ExpressionProductionIndex.String_Stringliteral:
                // <String> ::= StringLiteral
                return ((string)r[0].Data).Trim('"');

            case ExpressionProductionIndex.Program:
                // <Program> ::= <nlo> <Exp> <nlo>
                return r.PassOn(1);

            case ExpressionProductionIndex.Program2:
                // <Program> ::= <nlo>
                return new EmptyExpression();

            case ExpressionProductionIndex.Exp:
                // <Exp> ::= <AndOr Exp>
                return r.PassOn();

            case ExpressionProductionIndex.Andorexp_And:
                // <AndOr Exp> ::= <Is Exp> AND <AndOr Exp>

                return new AndExpression()
                {
                    Left = (Expression)r[0].Data,
                    Right = (Expression)r[2].Data
                };


            case ExpressionProductionIndex.Andorexp_Ampamp:
                // <AndOr Exp> ::= <Is Exp> '&&' <AndOr Exp>

                return new AndExpression()
                {
                    Left = (Expression)r[0].Data,
                    Right = (Expression)r[2].Data
                };


            case ExpressionProductionIndex.Andorexp_Or:
                // <AndOr Exp> ::= <Is Exp> OR <AndOr Exp>

                return new OrExpression()
                {
                    Left = (Expression)r[0].Data,
                    Right = (Expression)r[2].Data
                };

            case ExpressionProductionIndex.Andorexp_Pipepipe:
                // <AndOr Exp> ::= <Is Exp> '||' <AndOr Exp>

                return new OrExpression()
                {
                    Left = (Expression)r[0].Data,
                    Right = (Expression)r[2].Data
                };

            case ExpressionProductionIndex.Andorexp:
                // <AndOr Exp> ::= <Is Exp>
                return r.PassOn();

            case ExpressionProductionIndex.Isexp_Eqeq:
                // <Is Exp> ::= <Value Exp> '==' <Is Exp>

                return new EqualExpression()
                {
                    Left = (Expression)r[0].Data,
                    Right = (Expression)r[2].Data
                };

            case ExpressionProductionIndex.Isexp_Exclameq:
                // <Is Exp> ::= <Value Exp> '!=' <Is Exp>

                return new NotEqualExpression()
                {
                    Left = (Expression)r[0].Data,
                    Right = (Expression)r[2].Data
                };

            case ExpressionProductionIndex.Isexp:
                // <Is Exp> ::= <Value Exp>
                return r.PassOn();

            case ExpressionProductionIndex.Valueexp:
                // <Value Exp> ::= <Value>
                return r.PassOn();

            case ExpressionProductionIndex.Valueexp2:
                // <Value Exp> ::= <Function>
                return r.PassOn();

            case ExpressionProductionIndex.Valueexp3:
                // <Value Exp> ::= <IsRelated>
                return r.PassOn();

            case ExpressionProductionIndex.Valueexp4:
                // <Value Exp> ::= <IsNotRelated>
                return r.PassOn();

            case ExpressionProductionIndex.Valueexp_Lparen_Rparen:
                // <Value Exp> ::= '(' <Exp> ')'
                return new BracketsExpression()
                {
                    Expression = (Expression)r[1].Data
                };

            case ExpressionProductionIndex.Function_Lparen_Rparen:
                // <Function> ::= <String> '(' <Args> ')'

                return new FunctionExpression()
                {
                    Name = (string)r[0].Data,
                    Arguments = (ArgumentList)r[2].Data
                };

            case ExpressionProductionIndex.Args_Comma:
                // <Args> ::= <Args> ',' <Exp>
                {
                    var value = (ArgumentList)r[0].Data;

                    value.ExpressionList.Add((Expression)r[2].Data);

                    return value;
                }

            case ExpressionProductionIndex.Args:
                // <Args> ::= <Exp>

                return new ArgumentList()
                {
                    ExpressionList = [(Expression)r[0].Data]
                };

            case ExpressionProductionIndex.Args2:
                // <Args> ::= 

                return new ArgumentList();

            case ExpressionProductionIndex.Isrelated_Minusquestiongt:
                // <IsRelated> ::= <CompositeValue> '-?>' <CompositeValue>

                return new HasRelationExpression()
                {
                    Left = (CompositeValue)r[0].Data,
                    Right = (CompositeValue)r[2].Data
                };

            case ExpressionProductionIndex.Isrelated_Minusquestiongt_Colon:
                // <IsRelated> ::= <CompositeValue> '-?>' <CompositeValue> ':' <String>

                return new HasRelationExpression()
                {
                    Name = (string)r[4].Data,
                    Left = (CompositeValue)r[0].Data,
                    Right = (CompositeValue)r[2].Data
                };

            case ExpressionProductionIndex.Isnotrelated_Minusexclamgt:
                // <IsNotRelated> ::= <CompositeValue> '-!>' <CompositeValue>

                return new DoesNotHaveRelationExpression()
                {
                    Left = (CompositeValue)r[0].Data,
                    Right = (CompositeValue)r[2].Data
                };

            case ExpressionProductionIndex.Isnotrelated_Minusexclamgt_Colon:
                // <IsNotRelated> ::= <CompositeValue> '-!>' <CompositeValue> ':' <String>

                return new DoesNotHaveRelationExpression()
                {
                    Name = (string)r[4].Data,
                    Left = (CompositeValue)r[0].Data,
                    Right = (CompositeValue)r[2].Data
                };

            case ExpressionProductionIndex.Value:
                // <Value> ::= <CompositeValue>

                return r.PassOn();

            case ExpressionProductionIndex.Valuecomposite_Dot:
                // <CompositeValue> ::= <CompositeValue> '.' <String>
                {
                    var value = (CompositeValue)r[0].Data;

                    value.ValueList.Add((string)r[2].Data);

                    return value;
                }

            case ExpressionProductionIndex.Valuecomposite:
                // <CompositeValue> ::= <String>

                return new CompositeValue()
                {
                    ValueList = [(string)r[0].Data]
                };

            default:
                throw new NotImplementedException("Case not handled : " + (ExpressionProductionIndex)r.Parent.TableIndex());
        }

        throw new NotImplementedException("Case did not return value : " + (ExpressionProductionIndex)r.Parent.TableIndex());
    }

}
