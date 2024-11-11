using LanguageExt.Common;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public static class ResultExtensions
{
    public static void Switch<A>(this Result<A> r, Action<A> Succ, Action<Exception> Fail)
    {
        r.IfFail(Fail);
        r.IfSucc(Succ);
    }
}
