using Microsoft.EntityFrameworkCore;
using Picator.Common.Data.Dtos.Users;
using Picator.Data;
using Picator.Service.Contracts.Identity;

namespace Picator.Service.Services.Identity;

public class IdentityService : IIdentityService
{
    protected readonly ApplicationDbContext dbConnection;

    public IdentityService(ApplicationDbContext dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public async Task<IEnumerable<ValidateUserResult>> GetByUsername(string username)
    {
        return await dbConnection.Database.SqlQuery<ValidateUserResult>($"SELECT TOP 1 [Id],[DisplayName],[Image] FROM [Users] WHERE Username = {username}").ToListAsync();
    }
}