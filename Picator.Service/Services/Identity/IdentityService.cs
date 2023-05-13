using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Identity;
using RepoDb;
using System.Data;

namespace Picator.Service.Services.Identity;

public class IdentityService : IIdentityService
{
    protected readonly IDbConnection dbConnection;

    public IdentityService(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public async Task<IEnumerable<ValidateUserResult>> GetByUsername(string username)
    {
        return await dbConnection.ExecuteQueryAsync<ValidateUserResult>(
            "SELECT TOP 1 [Id],[DisplayName],[Image] FROM [Users] WHERE Username = @username", new { username });
    }
}