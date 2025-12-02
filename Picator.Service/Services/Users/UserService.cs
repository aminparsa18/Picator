using Microsoft.EntityFrameworkCore;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Data;
using Picator.Repository;
using Picator.Service.Contracts.Users;

namespace Picator.Service.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbConnection;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(ApplicationDbContext dbConnection, IUnitOfWork unitOfWork)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<UserDetailsResult>> GetDetails(string userId)
    {
        var user = await _dbConnection.Database.SqlQuery<UserDetailsResult>($"SELECT TOP 1 [Avatar],[Score],[DisplayName],[Email] FROM [Users] WHERE Id = {userId}").FirstOrDefaultAsync();
        if (user == null)
        {
            return new ApiResult<UserDetailsResult>()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = ["User does not exist"]
            };
        }
        user.Avatar = string.Join(Constants.BlobStorageEndpoint, user.Avatar);
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