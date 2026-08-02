using System.Text.Json.Serialization;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.GameMembers;
using Picator.Common.Data.Dtos.RoomMembers;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Benchmark;

/// <summary>
/// System.Text.Json source-generated serialization context, used so the JSON side of the
/// benchmark also skips reflection-based (de)serialization, matching MemoryPack's source-gen model.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(AuthResult))]
[JsonSerializable(typeof(UserDetailsResult))]
[JsonSerializable(typeof(ApiResult<List<GameMemberResult>>))]
[JsonSerializable(typeof(List<RoomMemberResult>))]
public sealed partial class PicatorJsonContext : JsonSerializerContext;
