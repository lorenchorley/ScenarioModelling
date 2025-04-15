using Excubo.Blazor.Canvas.Contexts;
using ScenarioModelling.CoreObjects;

namespace WebDesigner.Blazor.Client.Rendering;

public class Renderer(Vector2 size)
{
    public event EventHandler? OnDispose;

    public async Task<string> Render(MetaStory metaStory, Context2D canvas, Func<string, Task> debugLog)
    {
        try
        {
            RenderingQueue renderingQueue = new(debugLog); // TODO replace with a channel ?

            renderingQueue.Enqueue("Layout", async () =>
            {
                await canvas.FillStyleAsync("white");
                await canvas.ClearRectAsync(0, 0, size.X, size.Y);

                // Top left
                await canvas.FillStyleAsync("#FFFFFF");
                await canvas.FillRectAsync(0, 0, size.X / 2, 2 * size.Y / 3);

                // Bottom
                await canvas.FillStyleAsync("#00FF0020");
                await canvas.FillRectAsync(0, 2 * size.Y / 3, size.X, size.Y / 3);

                // Top right
                await canvas.FillStyleAsync("#0000FF20");
                await canvas.FillRectAsync(size.X / 2, 0, size.X / 2, 2 * size.Y / 3);

            });

            SemiLinearSubGraphRenderer _metaStoryGraphRenderer = new(renderingQueue, canvas, 0, 0);

            foreach (var item in metaStory.Graph.PrimarySubGraph.UnorderedEnumerable)
            {
                await item.Accept(_metaStoryGraphRenderer);
            }

            var lastDot = await _metaStoryGraphRenderer.DrawNextDot("Last dot", "black"); // Different design
            await _metaStoryGraphRenderer.DoToDos(lastDot);

            await renderingQueue.FinishAndContinueWith(() => OnDispose?.Invoke(this, EventArgs.Empty));

            //var _ = Task.Run(async () =>
            //{
            //    await Task.WhenAll(uiTasks);
            //    OnDispose?.Invoke(this, EventArgs.Empty);
            //});

            return await Task.FromResult($"");
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}");
        }
    }
}