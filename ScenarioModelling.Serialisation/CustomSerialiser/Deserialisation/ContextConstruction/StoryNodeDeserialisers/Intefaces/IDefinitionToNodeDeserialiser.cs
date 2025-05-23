﻿using LanguageExt;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;

public interface IDefinitionToNodeDeserialiser
{
    string Name { get; }
    Func<Definition, bool>? Predicate { get; }
    IStoryNode Transform(Definition def, MetaStory MetaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate tryTransform);
}

