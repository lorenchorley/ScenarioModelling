using System.Collections.Concurrent;

namespace WebDesigner.Blazor.Client.Rendering;

public class RenderingQueue(Func<string, Task> debugLog)
{
    private object _lock = new();
    private Task? _loopTask = null;
    private CancellationTokenSource canceller = new();
    private ConcurrentQueue<(string, Func<Task>)> _queue = new();

    public void Enqueue(string taskName, Func<Task> action)
    {
        lock(_lock)
        {
            if (canceller.Token.IsCancellationRequested)
            {
                throw new Exception("Cannot enqueue another operation, the rendering queue has already been finished");
            }

            if (_loopTask == null)
            {
                _loopTask = Task.Run(Loop);
            }

            _queue.Enqueue((taskName, action));
        }
    }
    
    private async Task Loop()
    {
        while (!canceller.Token.IsCancellationRequested || _queue.Count != 0)
        {
            if (_queue.TryDequeue(out (string, Func<Task>) task))
            {
                await task.Item2.Invoke();
                await debugLog.Invoke(task.Item1);
            }
        }
    }

    public async Task FinishAndContinueWith(Action callback)
    {
        lock (_lock)
        {
            canceller.Cancel();
        }

        if (_loopTask != null)
            await _loopTask;

        callback.Invoke();
    }
}
