using FluentValidation;
using Picator.Common.Data.Dtos.RoomMembers;

namespace Picator.Service.Validations.RoomMembers;

public class NewMembersRequestValidator : AbstractValidator<NewMembersRequest>
{
    public NewMembersRequestValidator()
    {
        RuleFor(x => x.RoomId).NotEqual(Guid.Empty);
        RuleFor(x => x.Users).ForEach(u => u.NotEqual(Guid.Empty));
    }
}