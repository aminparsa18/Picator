using Microsoft.AspNetCore.Identity;

namespace Picator.Entities.Identity;

/// <summary>
/// Role.
/// </summary>
public sealed class Role : IdentityRole<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class.
    /// </summary>
    public Role()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class.
    /// </summary>
    public Role(string name) : base(name)
    {
        Name = name;
    }

    /// <summary>
    /// Collection of users. 
    /// </summary>
    public ICollection<UserRole>? Users { get; set; }

    /// <summary>
    /// Collection of claims.
    /// </summary>
    public ICollection<RoleClaim>? Claims { get; set; }
}