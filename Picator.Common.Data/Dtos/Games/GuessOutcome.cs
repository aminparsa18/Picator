namespace Picator.Common.Data.Dtos.Games;

/// <summary>
/// Result of a guess submitted via IGameCreateService.SubmitGuess.
/// </summary>
/// <param name="WasCorrect">Whether the guess matched the round word.</param>
/// <param name="Word">The round word, revealed once the round ends.</param>
/// <param name="PointsAwarded">Points awarded to the guesser for this guess.</param>
/// <param name="RoundJustCompleted">True when this guess ended the round (broadcast <c>OnRoundEnded</c> to the group).</param>
public sealed record GuessOutcome(bool WasCorrect, string Word, int PointsAwarded, bool RoundJustCompleted);
