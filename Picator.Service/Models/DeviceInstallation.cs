using System.ComponentModel.DataAnnotations;

namespace Picator.Service.Models;

public class DeviceInstallation
{
    [Required]
    public string InstallationId { get; set; } = default!;

    [Required]
    public string Platform { get; set; } = default!;

    [Required]
    public string PushChannel { get; set; } = default!;

    public IList<string> Tags { get; set; } = Array.Empty<string>();
}