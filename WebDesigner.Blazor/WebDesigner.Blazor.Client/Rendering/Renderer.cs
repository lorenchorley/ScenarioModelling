using Excubo.Blazor.Canvas.Contexts;
using ScenarioModelling.CoreObjects;

namespace WebDesigner.Blazor.Client.Rendering;

public class Renderer(Vector2 size)
{
    public event EventHandler? OnDispose;

    public async Task<string> Render(MetaStory metaStory, Context2D canvasContext)
    {
        try
        {
            List<Task> uiTasks = new();

            await canvasContext.ClearRectAsync(0, 0, size.X, size.Y);
            //await canvasContext.FillRectAsync(size.X / 4, size.Y / 4, size.X / 2, size.Y / 2);

            SemiLinearSubGraphRenderer _metaStoryGraphRenderer = new(uiTasks, canvasContext, 0, 0);

            foreach (var item in metaStory.Graph.PrimarySubGraph.UnorderedEnumerable)
            {
                await item.Accept(_metaStoryGraphRenderer);
            }

            var lastDot = await _metaStoryGraphRenderer.DrawNextDot(); // Different design
            await _metaStoryGraphRenderer.DoToDos(lastDot);

            var _ = Task.Run(async () =>
            {
                await Task.WhenAll(uiTasks);
                OnDispose?.Invoke(this, EventArgs.Empty);
            });

            return await Task.FromResult($"");
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}");
        }
    }
}