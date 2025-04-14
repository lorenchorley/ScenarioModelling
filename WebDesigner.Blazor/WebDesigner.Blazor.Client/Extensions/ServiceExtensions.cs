using Microsoft.Extensions.DependencyInjection.Extensions;
using WebDesigner.Blazor.Client.Services;

namespace WebDesigner.Blazor.Client.Extensions;

public static class ServiceExtensions
{
    public static void AddJSInterop(this IServiceCollection services)
    {
        services.TryAddScoped<IJSInterop, JSInterop>();
    }
}
