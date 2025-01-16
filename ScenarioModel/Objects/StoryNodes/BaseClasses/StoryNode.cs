using Newtonsoft.Json;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Objects.Visitors;

namespace ScenarioModelling.Objects.StoryNodes.BaseClasses;

public abstract record StoryNode<E> : IStoryNode where E : IStoryEvent
{
    public string Name { get; set; } = "";

    [StoryNodeLikeProperty(optionalBool: OptionalBoolSetting.FalseAsDefault)]
    public bool Implicit { get; set; } = false;

    [JsonIgnore]
    public Type Type => typeof(E);
    public int? Line { get; set; }
    public abstract E GenerateEvent(EventGenerationDependencies dependencies);
    public abstract IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs();

    public IStoryEvent GenerateGenericTypeEvent(EventGenerationDependencies dependencies)
    {
        return GenerateEvent(dependencies);
    }

    public string? LineInformation
    {
        get => Line.HasValue ? $" (Near line {Line.Value})" : "";
    }

    public abstract OneOfIScenaroNode ToOneOf();
    public abstract object Accept(IMetaStoryVisitor visitor);
    public abstract bool IsFullyEqv(IStoryNode other);
}
