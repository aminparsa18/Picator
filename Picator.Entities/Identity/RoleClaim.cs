using Microsoft.AspNetCore.Identity;

namespace Picator.Entities.Identity;

/// <summary>
/// Role claim.
/// </summary>
public class RoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    /// Role.
    /// </summary>
    public virtual Role? Role { get; set; }
}