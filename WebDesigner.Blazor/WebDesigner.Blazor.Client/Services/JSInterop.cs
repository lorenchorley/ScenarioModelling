using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebDesigner.Blazor.Client.Services;

public delegate void NotifyResize();
public delegate void NotifyResizing(bool isResizing);

public class JSInterop : IJSInterop
{
    public BrowserSizeDetails BrowserSizeDetails { get; private set; } = new BrowserSizeDetails();

    private readonly IJSRuntime _jsRuntime;

    //private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly ILogger<JSInterop> _logger;
    private bool _isResizing;
    private System.Timers.Timer _resizeTimer;
    private DotNetObjectReference<JSInterop> _jsInteropCoreServiceRef;

    public event NotifyResizing OnResizing;
    public event NotifyResize OnResize;

    public JSInterop(IJSRuntime jsRuntime, ILogger<JSInterop> logger)
    {
        _resizeTimer = new System.Timers.Timer(interval: 25);
        _isResizing = false;
        _resizeTimer.Elapsed += DimensionsChanged;
        _jsRuntime = jsRuntime;
        //_moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(identifier: "import", args: "./js/interop.js").AsTask());
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        //IJSObjectReference module = await GetModuleAsync();

        _jsInteropCoreServiceRef = DotNetObjectReference.Create(this);

        await _jsRuntime.InvokeVoidAsync(identifier: "listenToWindowResize", _jsInteropCoreServiceRef);

        BrowserSizeDetails = await _jsRuntime.InvokeAsync<BrowserSizeDetails>(identifier: "getWindowSizeDetails");

    }

    public async Task DrawText(ElementReference canvasElement, string text)
    {
        _logger.LogDebug("Drawing text on canvas element");
        //IJSObjectReference module = await GetModuleAsync();
        await _jsRuntime.InvokeVoidAsync(identifier: "drawText", canvasElement, text);
    }

    public async Task<BrowserSizeDetails> GetWindowSizeAsync()
    {
        //IJSObjectReference module = await GetModuleAsync();
        return await _jsRuntime.InvokeAsync<BrowserSizeDetails>(identifier: "getWindowSizeDetails");
    }

    public async Task<ElementBoundingRectangle> GetElementBoundingRectangleAsync(ElementReference elementReference)
    {
        //IJSObjectReference module = await GetModuleAsync();
        return await _jsRuntime.InvokeAsync<ElementBoundingRectangle>(identifier: "getBoundingRectangle", elementReference);
    }

    [JSInvokable]
    public Task WindowResizeEvent()
    {
        if (_isResizing is not true)
        {
            _isResizing = true;
            OnResizing?.Invoke(_isResizing);
        }
        DebounceResizeEvent();
        return Task.CompletedTask;
    }

    private void DebounceResizeEvent()
    {
        if (_resizeTimer.Enabled is false)
        {
            Task.Run(async () =>
            {
                BrowserSizeDetails = await GetWindowSizeAsync();
                _isResizing = false;
                OnResizing?.Invoke(_isResizing);
                OnResize?.Invoke();
            });
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }
    }

    private async void DimensionsChanged(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _resizeTimer.Stop();
        BrowserSizeDetails = await GetWindowSizeAsync();
        _isResizing = false;
        OnResizing?.Invoke(_isResizing);
        OnResize?.Invoke();
    }

    //public async ValueTask DisposeAsync()
    //{
    //    if (_moduleTask.IsValueCreated)
    //    {
    //        IJSObjectReference module = await GetModuleAsync();
    //        await module.DisposeAsync();
    //    }
    //}

    //private async Task<IJSObjectReference> GetModuleAsync()
    //    => await _moduleTask.Value;

}