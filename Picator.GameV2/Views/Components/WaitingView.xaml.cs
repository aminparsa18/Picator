namespace Picator.GameV2.Views.Components;

public partial class WaitingView : ContentView
{
    public WaitingView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(WaitingView), false);

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly BindableProperty HasShadowProperty =
        BindableProperty.Create(nameof(HasShadow), typeof(bool), typeof(WaitingView), false);

    public bool HasShadow
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }
}