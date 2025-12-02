using Picator.Data.Dtos.User;

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
    public Task<List<RefreshTokenDetails>> GetByToken(string refreshToken)
    {
        return Context.Database.SqlQuery<RefreshTokenDetails>($"SELECT [r].[Id], [r].[ExpirationDate], [r].[IsInvalidated], [r].[IsUsed], [r].[JwtId] FROM[dbo].[RefreshToken] AS[r] WHERE [r].[Token] = {refreshToken}").ToListAsync();
    }

    /// <inheritdoc/>
    public Task<int> SetUsed(string id)
    {
        return Context.Database.ExecuteSqlAsync($"UPDATE [RefreshToken] SET [IsUsed] = 1 WHERE [Id] = {id}");
    }
}