using Picator.Common.Data.Dtos.Avatars;
using Picator.Repository;
using Picator.Service.Contracts.Avatars;

namespace Picator.Service.Services.Avatars;

public class AvatarService : IAvatarService
{
    private readonly IUnitOfWork _unitOfWork;

    public AvatarService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AvatarResult>> GetAll()
    {
        IEnumerable<AvatarResult> avatars = await _unitOfWork.Avatar.GetAllDtos();

        // Set full uri path on avatar name.
        foreach (AvatarResult avatar in avatars)
        {
            avatar.Name = Data.Constants.BlobStorageEndpoint + avatar.Name;
        }
        return avatars;
    }
}