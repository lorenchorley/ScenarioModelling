using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

public abstract record StoryNode : IStoryNode
{
    public string Name { get; set; } = "";

    [StoryNodeLikeProperty(optionalBool: OptionalBoolSetting.FalseAsDefault)]
    public bool Implicit { get; set; } = false;

    public Type Type { get => GetType(); }
    public int? Line { get; set; }
    public abstract IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs();

    public string? LineInformation
    {
        get => Line.HasValue ? $" (Near line {Line.Value})" : "";
    }

    public abstract OneOfMetaStoryNode ToOneOf();
    public abstract object Accept(IMetaStoryVisitor visitor);
    public abstract Task<object> Accept(IMetaStoryAsyncVisitor visitor);
    public abstract bool IsFullyEqv(IStoryNode other);
}
