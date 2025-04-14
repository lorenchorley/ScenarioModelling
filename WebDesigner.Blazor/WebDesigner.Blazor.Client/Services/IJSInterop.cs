using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace WebDesigner.Blazor.Client.Services;

public interface IJSInterop
{
    BrowserSizeDetails BrowserSizeDetails { get; }

    event NotifyResizing OnResizing;
    event NotifyResize OnResize;

    Task DrawText(ElementReference canvasElement, string text);
    Task<ElementBoundingRectangle> GetElementBoundingRectangleAsync(ElementReference elementReference);
    Task<BrowserSizeDetails> GetWindowSizeAsync();
    Task InitializeAsync();
}
