namespace Picator.Common.Data.Dtos.Games;

/// <summary>
/// Result of a player joining a game via IGameCreateService.JoinGame.
/// </summary>
/// <param name="IsDrawer">Whether the joining player is the drawer for the active round.</param>
/// <param name="Word">The round word, populated only for the drawer.</param>
/// <param name="WordLength">Length of the round word, populated once the round has started.</param>
/// <param name="JustCompletedLegacyPairing">
/// True when this join was the second join-by-code join that just created the round (legacy flow) —
/// the caller should push the word to the already-connected drawer via <see cref="DrawerWordToPush"/>.
/// </param>
/// <param name="DrawerWordToPush">The word to push to the already-connected drawer when <see cref="JustCompletedLegacyPairing"/> is true.</param>
/// <param name="RoundDurationSeconds">How long the active round's timer runs for, in seconds.</param>
public sealed record GameJoinOutcome(bool IsDrawer, string? Word, int WordLength, bool JustCompletedLegacyPairing, string? DrawerWordToPush, int RoundDurationSeconds);
