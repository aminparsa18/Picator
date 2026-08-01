namespace Picator.Game.Controls;

public class DotGridDrawable : IDrawable
{
    public Color DotColor { get; set; } = Colors.Black;
    public float Spacing { get; set; } = 18f;
    public float DotRadius { get; set; } = 1f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.FillColor = DotColor;

        for (var y = 0f; y <= dirtyRect.Height; y += Spacing)
        {
            for (var x = 0f; x <= dirtyRect.Width; x += Spacing)
            {
                canvas.FillCircle(x, y, DotRadius);
            }
        }

        canvas.RestoreState();
    }
}
