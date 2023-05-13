using Picator.Common.Data.Dtos.Avatars;
using Picator.Game.Models;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;

namespace Picator.Game.Mappers;

[Mapper]
public partial class AvatarMapper
{
    public partial IEnumerable<Avatar> AvatarResultToAvatar(IEnumerable<AvatarResult> avatarResult);
}