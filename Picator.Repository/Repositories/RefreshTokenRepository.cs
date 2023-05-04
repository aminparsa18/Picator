using Picator.Data;
using Picator.Data.Dtos.User;
using Picator.Entities.Models;
using Picator.Repository.Contracts;
using System.Data;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
    /// </summary>
    public RefreshTokenRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<IEnumerable<RefreshTokenDetails>> GetByToken(string refreshToken)
    {
        return Connection.ExecuteQueryAsync<RefreshTokenDetails>(@"SELECT [r].[Id], [r].[ExpirationDate], [r].[IsInvalidated], [r].[IsUsed], [r].[JwtId]
            FROM[dbo].[RefreshToken] AS[r]
            WHERE [r].[Token] = @refreshToken", new { refreshToken });
    }

    /// <inheritdoc/>
    public Task<int> SetUsed(string id)
    {
        return Connection.ExecuteNonQueryAsync("UPDATE [RefreshToken] SET [IsUsed] = 1 WHERE [Id] = @id", new { id });
    }
}