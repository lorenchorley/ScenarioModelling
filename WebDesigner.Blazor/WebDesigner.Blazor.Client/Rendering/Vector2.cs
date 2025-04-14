namespace WebDesigner.Blazor.Client.Rendering;

public record struct Vector2(int X, int Y)
{
    public static implicit operator (int x, int y)(Vector2 value)
    {
        return (value.X, value.Y);
    }

    public static implicit operator Vector2((int x, int y) value)
    {
        return new Vector2(value.x, value.y);
    }
}