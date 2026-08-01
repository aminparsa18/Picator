# Picator — Game Rules & Strategy

This is the gameplay manifest: how a match is found, how a room fills and starts, how a round plays out, and how points are scored. For the entities that back this (`Room`, `Game`, `GameMember`, `Round`, ...), see the "Domain model" section of [`README.md`](README.md).

## Entry points

From the game menu there are two ways to play:

- **Quick Match** — random 1-on-1, auto-paired by the server.
- **Play with Friends** — a private `Room`, invite-only via `Room.Code`.

## Quick Match (random 1-on-1)

Tapping Quick Match drops the player into a matchmaking queue. As soon as a second waiting player is available, the server pairs them and starts a private match immediately. Once paired, the match is locked — it is not a browsable room, and no third player can join mid-game. Round structure and scoring follow the "Solo" rules below (single opponent, everyone who isn't drawing is the guesser).

## Play with Friends (Room)

Creating a Room requires picking a **format** up front:

- **Solo** — 2 players.
- **Teams** — 4 players, 2 teams of 2.

Players join anytime via the room code (share/copy link, or paste the code) and can trickle in in any order — 2 or 4 people can be in the lobby at any time. The match does **not** start until the format's full headcount is reached (2 for Solo, 4 for Teams). Once full, the host starts the game (or it auto-starts). The format is fixed at creation and isn't switchable mid-lobby.

A `Room` stays alive after a `Game` ends, so the same group can start a rematch without re-inviting.

## Round structure

A `Game` is a sequence of `Round`s. Each `Round` has exactly one drawer. The drawer rotates round-robin through all players, `RoundsPerPlayer` times each.

In **Teams** mode, draw order explicitly alternates team-to-team regardless of join order — e.g. A1, B1, A2, B2, repeat — so no team ever drafts twice in a row.

## Who guesses

**Solo / Quick Match:** trivial — the one other player is the guesser.

**Teams (2v2):** the drawer's **own teammate sits out** that round — they can chat/cheer, but their guesses don't count. The **opposing team** guesses, using a relay:

1. One of the two opposing players (alternating who goes first each time their team is on defense) gets the full round timer to guess.
2. If they fail (wrong or timeout), their teammate gets a **fresh full timer** — not the remainder of the first one.
3. If both fail, the round ends scoreless and the word is revealed to everyone.

*Why the opposing team guesses, not the drawer's own team:* with teams capped at 2, "own team guesses" leaves exactly one possible guesser once the drawer is subtracted — there's nothing left to relay to. Opposing-team-guesses is the only structure where the relay mechanic is actually meaningful.

## Scoring

- The **guessing team** scores the primary points for a round — more if the first guesser gets it, less if it took the relay.
- The **drawer's team** also earns a small bonus when their word is guessed correctly — rewards clear drawing and gives the sitting-out teammate a reason to care about the round.
- Neither side scores if nobody guesses in time.
- At game end, totals (`GameMember.Score`, summed per team in Teams mode) determine the winner and roll into each player's lifetime `User.Score`.

## Design decisions

These were open forks during design; all are decided and reflected in the rules above:

1. **Drawer's teammate is excluded from guessing** that round, rather than allowed to guess unscored or scored like free-for-all.
2. **Relay timer is a fresh full timer**, not the remainder of the first guesser's clock.
3. **Drawer's team gets a small bonus** on a correct guess — scoring isn't zero-sum between the two teams.
4. **Draw order strictly alternates teams** each round, rather than following raw join order.
5. **Room format (Solo vs Teams) is chosen once at creation** and isn't switchable mid-lobby.
6. **Room lobbies fill freely up to capacity**; the game only starts once the full 2 (Solo) or 4 (Teams) required players are present.
