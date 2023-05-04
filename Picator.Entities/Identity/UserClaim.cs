using Microsoft.AspNetCore.Identity;

namespace Picator.Entities.Identity;

/// <summary>
/// User claim.
/// </summary>
public class UserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    /// User.
    /// </summary>
    public virtual User? User { get; set; }
}