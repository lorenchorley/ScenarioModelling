using ScenarioModel.Collections;
using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.Visitors;
using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.ScenarioNodes.BaseClasses;

public abstract record ScenarioNode<E> : IScenarioNode where E : IScenarioEvent
{
    public string Name { get; set; } = "";

    [JsonIgnore]
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
    public abstract object Accept(IScenarioVisitor visitor);
}
