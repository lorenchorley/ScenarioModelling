using ScenarioModelling.CoreObjects;

namespace WebDesigner.Blazor.Client.Rendering;

public class Renderer
{
    public event EventHandler? OnDispose;

    public async Task Render(MetaStory metaStory, List<VisualMetaStoryGraphElement> visualGraphElements, LogDelegate debugLog)
    {
        try
        {
            SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(visualGraphElements, debugLog, 0);

            foreach (var item in metaStory.Graph.PrimarySubGraph.UnorderedEnumerable)
            {
                await item.Accept(metaStoryGraphRenderer);
            }

            OnDispose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            await debugLog($"Error: {ex.Message}", OutputItemType.Error);
        }
    }
}