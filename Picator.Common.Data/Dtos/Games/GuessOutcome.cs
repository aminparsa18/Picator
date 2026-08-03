namespace Picator.Common.Data.Dtos.Games;

/// <summary>
/// Result of a guess submitted via IGameCreateService.SubmitGuess, or a round timing out via TimeoutRound.
/// </summary>
/// <param name="WasCorrect">Whether the guess matched the round word. Always false for a timeout.</param>
/// <param name="Word">The round word, revealed once the round ends.</param>
/// <param name="PointsAwarded">Points awarded to the guesser for this guess. 0 for a timeout.</param>
/// <param name="RoundJustCompleted">True when this call ended the round (broadcast <c>OnRoundEnded</c> to the group).</param>
/// <param name="GameCompleted">True when the round that just ended was the game's last round.</param>
/// <param name="DrawerScore">The just-ended round's drawer's current total score.</param>
/// <param name="GuesserScore">The just-ended round's guesser's current total score.</param>
public sealed record GuessOutcome(bool WasCorrect, string Word, int PointsAwarded, bool RoundJustCompleted, bool GameCompleted, int DrawerScore, int GuesserScore);
