using Excubo.Blazor.Canvas.Contexts;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.Visitors;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace WebDesigner.Blazor.Client.Rendering;

public class SemiLinearSubGraphRenderer(RenderingQueue renderingQueue, Context2D canvas, int horizontalOffset, int verticalOffset) : IMetaStoryAsyncVisitor
{
    private int _margin = 5;
    private int _usedVerticalSpace = 0;
    private int _verticalSpacing = 10;
    private int _dotSize = 10;

    public Vector2? LastPointDrawn { get; private set; }

    private Queue<Func<Vector2, Task>> ToDoOnNextDraw = new();

    public async Task DoToDos(Vector2 point)
    {
        while (ToDoOnNextDraw.TryDequeue(out var callback))
        {
            await callback.Invoke(point);
        }
    }

    public int GetVerticalSpaceUsed()
    {
        return _usedVerticalSpace - _verticalSpacing;
    }

    public async Task<Vector2> DrawNextDot(string type, string colour)
    {
        int x = _margin + horizontalOffset;
        int y = _margin + verticalOffset + _usedVerticalSpace;
        renderingQueue.Enqueue($"Draw {type} in {colour}", async () =>
        {
            await canvas.FillStyleAsync(colour);
            await canvas.FillRectAsync(x, y, _dotSize, _dotSize);
        });
        //await canvasContext.EllipseAsync(_margin, _margin + _position, _dotSize, _dotSize, 0, 0, 360);

        _usedVerticalSpace += (_dotSize + _verticalSpacing);

        LastPointDrawn = new Vector2(x + _dotSize / 2, y + _dotSize / 2);

        return LastPointDrawn.Value;
    }

    public async Task<object> VisitAssert(AssertNode assertNode)
    {
        var point = await DrawNextDot("Assert", "green");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitCallMetaStory(CallMetaStoryNode callMetaStory)
    {
        var point = await DrawNextDot("Call", "yellow");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitChoose(ChooseNode chooseNode)
    {
        var point = await DrawNextDot("Choose", "grey");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitDialog(DialogNode dialogNode)
    {
        var point = await DrawNextDot("Dialog", "black");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitIf(IfNode ifNode)
    {
        var point = await DrawNextDot("if", "cyan");

        await DoToDos(point);

        SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(renderingQueue, canvas, horizontalOffset + 20, verticalOffset + _usedVerticalSpace);

        foreach (var node in ifNode.SubGraph.NodeSequence)
        {
            await node.Accept(metaStoryGraphRenderer);
        }

        _usedVerticalSpace += metaStoryGraphRenderer.GetVerticalSpaceUsed() + _verticalSpacing;
        var lastSubgraphPoint = metaStoryGraphRenderer.LastPointDrawn;

        renderingQueue.Enqueue("down line", async () =>
        {
            await canvas.BeginPathAsync();
            await canvas.MoveToAsync(point.X, point.Y);
            await canvas.LineToAsync(point.X, point.Y + 15);
            await canvas.StrokeAsync();
        });

        renderingQueue.Enqueue("down right line", async () =>
        {
            await canvas.BeginPathAsync();
            await canvas.MoveToAsync(point.X, point.Y);
            await canvas.LineToAsync(point.X + 10, point.Y + 10);
            await canvas.StrokeAsync();
        });

        ToDoOnNextDraw.Enqueue(async (currentPoint) => {
            // Draw lines from if node to next node and the end of the subgraph to the next node
            //renderingQueue.Enqueue("line", async () =>
            //{
            //    await canvas.BeginPathAsync();
            //    await canvas.MoveToAsync(point.X, point.Y);
            //    await canvas.LineToAsync(currentPoint.X, currentPoint.Y);
            //    await canvas.StrokeAsync();
            //});

            //if (lastSubgraphPoint != null)
            //{
            //    renderingQueue.Enqueue("line", async () =>
            //    {
            //        await canvas.BeginPathAsync();
            //        await canvas.MoveToAsync(lastSubgraphPoint.Value.X, lastSubgraphPoint.Value.Y);
            //        await canvas.LineToAsync(currentPoint.X, currentPoint.Y);
            //        await canvas.StrokeAsync();
            //    });
            //}

        });

        return this;
    }

    public async Task<object> VisitJump(JumpNode jumpNode)
    {
        var point = await DrawNextDot("Jump", "orange");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitLoopNode(LoopNode loopNode)
    {
        var point = await DrawNextDot("Loop", "cyan");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitMetadata(MetadataNode metadataNode)
    {
        var point = await DrawNextDot("Metadata", "white");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitTransition(TransitionNode transitionNode)
    {
        var point = await DrawNextDot("Transition", "yellow");

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitWhile(WhileNode whileNode)
    {
        var point = await DrawNextDot("While", "cyan");

        await DoToDos(point);

        SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(renderingQueue, canvas, horizontalOffset + 20, verticalOffset + _usedVerticalSpace);

        foreach (var node in whileNode.SubGraph.NodeSequence)
        {
            await node.Accept(metaStoryGraphRenderer);
        }

        _usedVerticalSpace += metaStoryGraphRenderer.GetVerticalSpaceUsed() + _verticalSpacing;
        var lastSubgraphPoint = metaStoryGraphRenderer.LastPointDrawn;

        renderingQueue.Enqueue("down line", async () =>
        {
            await canvas.BeginPathAsync();
            await canvas.MoveToAsync(point.X, point.Y);
            await canvas.LineToAsync(point.X, point.Y + 30);
            await canvas.StrokeAsync();
        });

        renderingQueue.Enqueue("down right line", async () =>
        {
            await canvas.BeginPathAsync();
            await canvas.MoveToAsync(point.X, point.Y);
            await canvas.LineToAsync(point.X + 10, point.Y + 10);
            await canvas.StrokeAsync();
        });

        renderingQueue.Enqueue("down left line", async () =>
        {
            await canvas.BeginPathAsync();
            await canvas.MoveToAsync(point.X + 10, point.Y + 10);
            await canvas.LineToAsync(point.X, point.Y + 20);
            await canvas.StrokeAsync();
        });

        ToDoOnNextDraw.Enqueue(async (currentPoint) => {
            // Draw lines from if node to next node and the end of the subgraph to the next node
            //renderingQueue.Enqueue("line", async () =>
            //{
            //    await canvas.BeginPathAsync();
            //    await canvas.MoveToAsync(point.X, point.Y);
            //    await canvas.LineToAsync(currentPoint.X, currentPoint.Y);
            //    await canvas.StrokeAsync();
            //});

            //if (lastSubgraphPoint != null)
            //{
            //    renderingQueue.Enqueue("line", async () =>
            //    {
            //        await canvas.BeginPathAsync();
            //        await canvas.MoveToAsync(lastSubgraphPoint.Value.X, lastSubgraphPoint.Value.Y);
            //        await canvas.LineToAsync(point.X, point.Y);
            //        await canvas.StrokeAsync();
            //    });
            //}
        });

        return this;
    }

}
