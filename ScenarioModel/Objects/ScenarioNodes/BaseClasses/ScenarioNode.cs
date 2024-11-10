using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public abstract record ScenarioNode<E> : IScenarioNode where E : IScenarioEvent
{
    public string Name { get; set; } = "";
    public Type Type => typeof(E);
    public int? Line { get; set; }
    public abstract E GenerateEvent(EventGenerationDependencies dependencies);
    public abstract IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs();

    public IScenarioEvent GenerateUntypedEvent(EventGenerationDependencies dependencies)
    {
        return GenerateEvent(dependencies);
    }

    public string? LineInformation
    {
        get => Line.HasValue ? $" (Near line {Line.Value})" : "";
    }

    public abstract OneOfIScenaroNode ToOneOf();
}
