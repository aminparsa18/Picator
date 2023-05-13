namespace Picator.Game.Models;

public partial class Avatar : ObservableObject
{
    public string Name { get; set; }

    [ObservableProperty]
    private double _scale = 1;
}