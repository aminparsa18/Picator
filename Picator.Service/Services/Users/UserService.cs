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
        var user = await _dbConnection.Users
            .Where(u => u.Id == Guid.Parse(userId))
            .Select(u => new UserDetailsResult { Avatar = u.Avatar, Score = u.Score, DisplayName = u.DisplayName, Email = u.Email })
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return new ApiResult<UserDetailsResult>()
            {
                StatusCode = ApiResultStatusCode.Unauthorized,
                Errors = ["User does not exist"]
            };
        }
        user.Avatar = Constants.BlobStorageEndpoint + user.Avatar;
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