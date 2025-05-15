using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.Visitors;

namespace WebDesigner.Blazor.Client.Rendering;

public class SemiLinearSubGraphRenderer(List<VisualMetaStoryGraphElement> visualGraphElements, LogDelegate debugLog, int indent) : IMetaStoryAsyncVisitor
{
    public async Task DrawNextDot(string type, string text)
    {
        await debugLog($"Draw {type} with text {text} and indent {indent}");
        visualGraphElements.Add(new(type, text, indent));
    }

    public async Task<object> VisitAssert(AssertNode assertNode)
    {
        await DrawNextDot("Assert", assertNode.Name);

        return this;
    }

    public async Task<object> VisitCallMetaStory(CallMetaStoryNode callMetaStory)
    {
        await DrawNextDot("Call", $"Call {callMetaStory.MetaStoryName}");

        return this;
    }

    public async Task<object> VisitChoose(ChooseNode chooseNode)
    {
        await DrawNextDot("Choose", chooseNode.Name);

        return this;
    }

    public async Task<object> VisitDialog(DialogNode dialogNode)
    {
        await DrawNextDot("Dialog", dialogNode.TextTemplate);

        return this;
    }

    public async Task<object> VisitIf(IfNode ifNode)
    {
        await DrawNextDot("if", $"If {ifNode.OriginalConditionText}");

        SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(visualGraphElements, debugLog, indent + 1);

        foreach (var node in ifNode.SubGraph.NodeSequence)
        {
            await node.Accept(metaStoryGraphRenderer);
        }

        return this;
    }

    public async Task<object> VisitJump(JumpNode jumpNode)
    {
        await DrawNextDot("Jump", $"Jump to {jumpNode.Target}");

        return this;
    }

    public async Task<object> VisitLoopNode(LoopNode loopNode)
    {
        await DrawNextDot("Loop", "Loop");

        return this;
    }

    public async Task<object> VisitMetadata(MetadataNode metadataNode)
    {
        await DrawNextDot("Metadata", $"{metadataNode.Name} -> {metadataNode.Key}");

        return this;
    }

    public async Task<object> VisitTransition(TransitionNode transitionNode)
    {
        await DrawNextDot("Transition", $"{transitionNode.StatefulObject?.Name ?? "Object not set"} : {transitionNode.TransitionName}");

        return this;
    }

    public async Task<object> VisitWhile(WhileNode whileNode)
    {
        await DrawNextDot("While", $"While {whileNode.OriginalConditionText}");

        SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(visualGraphElements, debugLog, indent + 1);

        foreach (var node in whileNode.SubGraph.NodeSequence)
        {
            await node.Accept(metaStoryGraphRenderer);
        }

        return this;
    }

}
