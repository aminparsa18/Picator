using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts;
using Picator.Service.Contracts.Identity;
using RepoDb;
using System.Data;

namespace Picator.Service.Services.Identity;

public class IdentityService : IIdentityService
{
    protected readonly IDbConnection dbConnection;
    private readonly ISmsSender smsSender;

    public IdentityService(IDbConnection dbConnection, ISmsSender smsSender)
    {
        this.dbConnection = dbConnection;
        this.smsSender = smsSender;
    }

    public async Task<IEnumerable<ValidateUserResult>> GetByUsername(string username)
    {
        return await dbConnection.ExecuteQueryAsync<ValidateUserResult>(
            "SELECT TOP 1 [Id],[DisplayName],[Image] FROM [Users] WHERE Username = @username", new { username });
    }
}