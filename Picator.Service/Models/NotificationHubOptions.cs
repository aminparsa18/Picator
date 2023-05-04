using System.ComponentModel.DataAnnotations;

namespace Picator.Service.Models;

public class NotificationHubOptions
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string ConnectionString { get; set; } = default!;
}