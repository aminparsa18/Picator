using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Picator.Common.Extensions;
using Picator.Entities.Identity;
using Picator.Service.Contracts.Identity;

namespace Picator.Service.Services.Identity;

public class ApplicationRoleManager : RoleManager<Role>, IApplicationRoleManager
{
    private readonly IdentityErrorDescriber _errors;
    private readonly IIdentityService _userManager;
    private readonly ILookupNormalizer _keyNormalizer;
    private readonly ILogger<ApplicationRoleManager> _logger;
    private readonly IEnumerable<IRoleValidator<Role>> _roleValidators;
    private readonly IRoleStore<Role> _store;

    public ApplicationRoleManager(
        IRoleStore<Role> store,
        ILookupNormalizer keyNormalizer,
        ILogger<ApplicationRoleManager> logger,
        IEnumerable<IRoleValidator<Role>> roleValidators,
        IdentityErrorDescriber errors,
        IIdentityService userManager) :
        base(store, roleValidators, keyNormalizer, errors, logger)
    {
        _errors = errors;
        _errors.CheckArgumentIsNull(nameof(_errors));

        _keyNormalizer = keyNormalizer;
        _keyNormalizer.CheckArgumentIsNull(nameof(_keyNormalizer));

        _logger = logger;
        _logger.CheckArgumentIsNull(nameof(_logger));

        _store = store;
        _store.CheckArgumentIsNull(nameof(_store));

        _roleValidators = roleValidators;
        _roleValidators.CheckArgumentIsNull(nameof(_roleValidators));

        _userManager = userManager;
        _userManager.CheckArgumentIsNull(nameof(_userManager));
    }


    public Task<Role> FindByIdAsync(Guid roleId)
    {
        throw new NotImplementedException();
    }

    public List<Role> GetAllRoles()
    {
        return Roles.ToList();
    }
    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await Roles.ToListAsync();
    }



    public Task<Role> FindClaimsInRole(Guid roleId)
    {
        return Roles.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == roleId);
    }


    public async Task<IdentityResult> AddOrUpdateClaimsAsync(Guid roleId, string roleClaimType, IList<string> selectedRoleClaimValues)
    {
        var role = await FindClaimsInRole(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "NotFound",
                Description = "Role not found.",
            });
        }

        var currentRoleClaimValues = role.Claims.Where(r => r.ClaimType == roleClaimType).Select(r => r.ClaimValue).ToList();
        selectedRoleClaimValues ??= new List<string>();

        var newClaimValuesToAdd = selectedRoleClaimValues.Except(currentRoleClaimValues).ToList();
        foreach (var claim in newClaimValuesToAdd)
        {
            role.Claims.Add(new RoleClaim
            {
                RoleId = roleId,
                ClaimType = roleClaimType,
                ClaimValue = claim,
            });
        }

        var removedClaimValues = currentRoleClaimValues.Except(selectedRoleClaimValues).ToList();
        foreach (var roleClaim in removedClaimValues.Select(claim => role.Claims.SingleOrDefault(r => r.ClaimValue == claim && r.ClaimType == roleClaimType))
                     .Where(roleClaim => roleClaim != null))
        {
            role.Claims.Remove(roleClaim);
        }

        return await UpdateAsync(role);
    }
}