namespace Picator.GameV2.Services.Audio;

/// <summary>
/// Attaches a click sound to every Button and TapGestureRecognizer anywhere in the visual
/// tree below the given root, as they're added, so individual pages/controls don't need to
/// wire this up themselves. Whether the sound actually plays is governed by
/// <see cref="App.IsSoundEffectsOn"/> at the moment of the tap.
/// </summary>
public static class ButtonSoundHook
{
    private static readonly BindableProperty HookedProperty = BindableProperty.CreateAttached(
        "ClickSoundHooked", typeof(bool), typeof(ButtonSoundHook), false);

    public static void Attach(Element root)
    {
        root.DescendantAdded += (_, e) => Hook(e.Element);
        Hook(root);
    }

    private static void Hook(Element element)
    {
        if ((bool)element.GetValue(HookedProperty))
            return;
        element.SetValue(HookedProperty, true);

        if (element is Button button)
            button.Clicked += (_, _) => PlayClickSound();

        if (element is View { GestureRecognizers.Count: > 0 } view)
        {
            foreach (var recognizer in view.GestureRecognizers.OfType<TapGestureRecognizer>())
                recognizer.Tapped += (_, _) => PlayClickSound();
        }
    }

    private static void PlayClickSound() => (Application.Current as App)?.PlayClickSound();
}
