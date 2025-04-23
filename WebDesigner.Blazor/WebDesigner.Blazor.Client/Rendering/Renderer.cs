using ScenarioModelling.CoreObjects;

namespace WebDesigner.Blazor.Client.Rendering;

public class Renderer
{
    public event EventHandler? OnDispose;

    public async Task<string> Render(MetaStory metaStory, List<VisualGraphElement> visualGraphElements, Func<string, Task> debugLog)
    {
        try
        {
            SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(visualGraphElements, debugLog, 0);

            foreach (var item in metaStory.Graph.PrimarySubGraph.UnorderedEnumerable)
            {
                await item.Accept(metaStoryGraphRenderer);
            }

            OnDispose?.Invoke(this, EventArgs.Empty);

            return await Task.FromResult($"");
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}");
        }
    }
}