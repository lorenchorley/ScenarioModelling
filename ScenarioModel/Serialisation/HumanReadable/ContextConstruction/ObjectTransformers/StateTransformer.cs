using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, State>]
public class StateTransformer(System System, Instanciator Instanciator) : IDefinitionToObjectTransformer<State, StateReference>
{
    /// <summary>
    /// Should return a reference. The instance should be recorded to the system by the class itself
    /// </summary>
    /// <param name="def"></param>
    /// <returns></returns>
    public Option<StateReference> Transform(Definition def)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("State"))
        {
            return null;
        }

        State value = Instanciator.New<State>(definition: def);

        return value.GenerateReference();
    }

    public void Validate(State obj)
    {
    }
}

