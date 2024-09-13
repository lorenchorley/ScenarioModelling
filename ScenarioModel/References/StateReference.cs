using LanguageExt;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.References;

public class StateReference : IReference<State>
{
    public Option<State> ResolveReference(System system)
    {
        throw new NotImplementedException();
    }
}
