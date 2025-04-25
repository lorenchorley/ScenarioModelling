namespace WebDesigner.Blazor.Client.Rendering;

public enum OutputItemType
{
    Info,
    Warning,
    Error
}
public record OutputItem(string Text, OutputItemType Type);
