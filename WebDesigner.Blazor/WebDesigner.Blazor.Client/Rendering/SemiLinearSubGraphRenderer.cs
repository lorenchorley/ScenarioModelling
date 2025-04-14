using Excubo.Blazor.Canvas.Contexts;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.Visitors;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace WebDesigner.Blazor.Client.Rendering;

public class SemiLinearSubGraphRenderer(List<Task> uiTasks, Context2D canvas, int horizontalOffset, int verticalOffset, int margin = 5) : IMetaStoryAsyncVisitor
{
    private int _position = 0;
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
        return _position - _verticalSpacing;
    }

    public async Task<Vector2> DrawNextDot()
    {
        int x = margin + horizontalOffset;
        int y = margin + verticalOffset + _position;
        ParallelInteropCmd(async () =>
        {
            await canvas.FillRectAsync(x, y, _dotSize, _dotSize);
        });
        //await canvasContext.EllipseAsync(_margin, _margin + _position, _dotSize, _dotSize, 0, 0, 360);

        _position += (_dotSize + _verticalSpacing);

        LastPointDrawn = new Vector2(x + _dotSize / 2, y + _dotSize / 2);

        return LastPointDrawn.Value;
    }

    public async Task<object> VisitAssert(AssertNode assertNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitCallMetaStory(CallMetaStoryNode callMetaStory)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitChoose(ChooseNode chooseNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitDialog(DialogNode dialogNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitIf(IfNode ifNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(uiTasks, canvas, horizontalOffset + 20, _position + _verticalSpacing + _dotSize, 0);

        foreach (var node in ifNode.SubGraph.NodeSequence)
        {
            await node.Accept(metaStoryGraphRenderer);
        }

        _position += metaStoryGraphRenderer.GetVerticalSpaceUsed();
        var lastSubgraphPoint = metaStoryGraphRenderer.LastPointDrawn;

        ToDoOnNextDraw.Enqueue(async (currentPoint) => {
            // Draw lines from if node to next node and the end of the subgraph to the next node
            ParallelInteropCmd(async () =>
            {
                await canvas.BeginPathAsync();
                await canvas.MoveToAsync(point.X, point.Y);
                await canvas.LineToAsync(currentPoint.X, currentPoint.Y);
                await canvas.StrokeAsync();
            });

            if (lastSubgraphPoint != null)
            {
                ParallelInteropCmd(async () =>
                {
                    await canvas.BeginPathAsync();
                    await canvas.MoveToAsync(lastSubgraphPoint.Value.X, lastSubgraphPoint.Value.Y);
                    await canvas.LineToAsync(currentPoint.X, currentPoint.Y);
                    await canvas.StrokeAsync();
                });
            }

        });

        return this;
    }

    public async Task<object> VisitJump(JumpNode jumpNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitLoopNode(LoopNode loopNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitMetadata(MetadataNode metadataNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitTransition(TransitionNode transitionNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        return this;
    }

    public async Task<object> VisitWhile(WhileNode whileNode)
    {
        var point = await DrawNextDot();

        await DoToDos(point);

        SemiLinearSubGraphRenderer metaStoryGraphRenderer = new(uiTasks, canvas, horizontalOffset + 20, _position + _verticalSpacing + _dotSize, 0);

        foreach (var node in whileNode.SubGraph.NodeSequence)
        {
            await node.Accept(metaStoryGraphRenderer);
        }

        _position += metaStoryGraphRenderer.GetVerticalSpaceUsed();
        var lastSubgraphPoint = metaStoryGraphRenderer.LastPointDrawn;

        ToDoOnNextDraw.Enqueue(async (currentPoint) => {
            // Draw lines from if node to next node and the end of the subgraph to the next node
            ParallelInteropCmd(async () =>
            {
                await canvas.BeginPathAsync();
                await canvas.MoveToAsync(point.X, point.Y);
                await canvas.LineToAsync(currentPoint.X, currentPoint.Y);
                await canvas.StrokeAsync();
            });

            if (lastSubgraphPoint != null)
            {
                ParallelInteropCmd(async () =>
                {
                    await canvas.BeginPathAsync();
                    await canvas.MoveToAsync(lastSubgraphPoint.Value.X, lastSubgraphPoint.Value.Y);
                    await canvas.LineToAsync(point.X, point.Y);
                    await canvas.StrokeAsync();
                });
            }
        });

        return this;
    }

    private void ParallelInteropCmd(Func<Task> func)
    {
        uiTasks.Add(Task.Run(func));
    }
}
