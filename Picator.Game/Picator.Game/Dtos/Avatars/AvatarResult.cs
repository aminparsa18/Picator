using MemoryPack;

namespace Picator.Common.Data.Dtos.Avatars;

[MemoryPackable]
public sealed partial class AvatarResult
{
    public string? Name { get; set; }
}