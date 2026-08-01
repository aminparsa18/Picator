namespace Picator.Game.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DotGridBackground : ContentView
{
    public static readonly BindableProperty DotColorProperty = BindableProperty.Create(
        nameof(DotColor),
        typeof(Color),
        typeof(DotGridBackground),
        Colors.Black,
        propertyChanged: OnAppearancePropertyChanged);

    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(DotGridBackground),
        18d,
        propertyChanged: OnAppearancePropertyChanged);

    public static readonly BindableProperty DotRadiusProperty = BindableProperty.Create(
        nameof(DotRadius),
        typeof(double),
        typeof(DotGridBackground),
        1d,
        propertyChanged: OnAppearancePropertyChanged);

    public Color DotColor
    {
        get => (Color)GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public double DotRadius
    {
        get => (double)GetValue(DotRadiusProperty);
        set => SetValue(DotRadiusProperty, value);
    }

    private readonly DotGridDrawable drawable = new();

    public DotGridBackground()
    {
        InitializeComponent();
        canvas.Drawable = drawable;
        ApplyToDrawable();
    }

    private static void OnAppearancePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (DotGridBackground)bindable;
        view.ApplyToDrawable();
        view.canvas.Invalidate();
    }

    private void ApplyToDrawable()
    {
        drawable.DotColor = DotColor;
        drawable.Spacing = (float)Spacing;
        drawable.DotRadius = (float)DotRadius;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        canvas.Invalidate();
    }
}
