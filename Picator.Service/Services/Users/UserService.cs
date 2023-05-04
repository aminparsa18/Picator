using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Repository;
using Picator.Service.Contracts.Users;
using RepoDb;
using System.Data;

namespace Picator.Service.Services.Users;
public class UserService : IUserService
{
    private readonly IDbConnection _dbConnection;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IDbConnection dbConnection, IUnitOfWork unitOfWork)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<UserDetailsResult>> GetDetails(string userId)
    {
        var users = await _dbConnection.ExecuteQueryAsync<UserDetailsResult>(
            "SELECT TOP 1 [Image],[Score],[DisplayName],[CountryCode] FROM [Users] WHERE Id = @id",
            new { id = userId });
        if (!users.Any())
        {
            return new ApiResult<UserDetailsResult>()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = new[] { "User does not exist" }
            };
        }
        var user = users.FirstOrDefault();
        user.Avatar = string.Join(Data.Constants.BlobStorageEndpoint, user.Avatar);
        return new ApiResult<UserDetailsResult>()
        {
            IsSuccess = true,
            Data = user
        };
    }

    public async Task<ApiResult<UserStatusResult>> GetStatus(string userId)
    {
       // var res = await _unitOfWork.GameMember.GetUserStatusFast(userId);
        var status = new UserStatusResult();
        return new ApiResult<UserStatusResult>
        {
            IsSuccess = true,
            Data = status
        };
    }
}