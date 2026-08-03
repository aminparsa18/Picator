# Picator

Picator is an online multiplayer Pictionary-style game: one player draws a word on a shared board in real time while the others guess what it is.

## Status

Early-stage. The domain model (this doc's focus) is in reasonable shape; the service/repository/hub layers are a partial scaffold — several methods are stubbed or commented out, EF migrations are stale relative to the model, and the `DbContext` isn't currently wired into DI. See "Known gaps" below.

## Architecture

.NET solution, split into layered projects:

| Project | Responsibility |
|---|---|
| `Picator.Entities` | Domain models (`Models/`) and EF Core mapping configuration (`Mapping/`) |
| `Picator.Common.Data` | Shared enums, constants, and DTOs used across layers |
| `Picator.Common` / `Picator.Common.Server` | Shared helpers/extensions and server-side auth helpers |
| `Picator.Data` | `ApplicationDbContext` and EF Core migrations |
| `Picator.Repository` | Repository / unit-of-work layer over the `DbContext` |
| `Picator.Service` | Application services, validation, and SignalR hubs (`RoomHub`, `ChatHub`, `GameHub`) |
| `Picator.Api` | ASP.NET Core minimal API endpoints |
| `Picator.Realtime` / `Picator.Realtime.Common` | Separate real-time (MagicOnion) service for drawing/streaming |
| `Picator.GameV2` | .NET MAUI client app |
| `Picator.Invitement/*` | Blazor invitation web app + client |
| `Picator.ExternalAuth` | External auth/token services |
| `Picator.Configuration` | Startup/DI wiring, middleware extensions |
| `Picator.AppHost` / `Picator.ServiceDefaults` | .NET Aspire orchestration host |

## Game strategy

See [`GAME_RULES.md`](GAME_RULES.md) for the full gameplay manifest — matchmaking flow, room formats, round/turn structure, the team guessing/relay mechanic, and scoring.

**Out of the relational model by design:** live drawing strokes are not persisted as rows — that's real-time transport (`GameHub`/SignalR/MagicOnion), broadcast to the group keyed by game/room id. The database only needs outcomes (who drew, what word, who guessed, what score), not the pixel/vector stream.

## Domain model

- `User` — player identity; `Score` is the lifetime total across all games.
- `Room` — persistent lobby. `Code` + `IsPrivate` gate joining; owned by a `User`; has many `Game`s (rematches) and `RoomMember`s.
- `RoomMember` — join table between `Room` and `User`.
- `Game` — one match instance against a `Room` (nullable `RoomId` — quick-match games may have none). `Status` (`GameStatus`), `RoundsPerPlayer`, `RoundDurationSeconds`, `GameCode`.
- `GameMember` — a player's participation in a `Game`. `Score` (this-game points), `Status` (`PlayerStatus`: Playing/Winner/Loser).
- `Round` — one player's drawing turn within a `Game`: `RoundNumber`, `DrawerGameMemberId`, `Word` (snapshot), `Status` (`RoundStatus`), `StartedAt`/`EndedAt`.
- `GameMessage` — chat and guesses in one stream. `RoundId`/`IsCorrectGuess`/`PointsAwarded` are set only for guess events.
- `GameWord` — flat word bank, picked from at random; intentionally has no relationship back to `Game`/`Round`.
- `Avatar` — avatar catalog (filename only; `User.Avatar` currently stores a string, not yet a strict FK).
- `GameConstants.MaxPlayers` (`Picator.Common.Data`) — fixed cap (4) on players per room/game; not a configurable per-row column.

## Known gaps

- `AddDbContext<ApplicationDbContext>` is commented out in `Picator.Configuration/Extensions/ServiceCollectionExtentions.cs` — the `DbContext` isn't wired into DI yet, despite the Aspire `AppHost` already provisioning and referencing a SQL Server resource. Should be reconnected via the Aspire integration pattern (`aspireify`), not a manual `UseSqlServer` call.
- EF Core migrations (`Picator.Data/Migrations`) haven't been regenerated since May 2023; the model has drifted since. A new migration will need to capture several years of accumulated changes at once, not just the latest round/scoring additions.
- `GameCreateService.Create()` is a stub (mapper calls commented out); `CreateMatchedGame` (quick-match, called from matchmaking pairing) and `JoinGame`/`SubmitGuess` (join-by-code + guess/round-completion, called from `GameHub`) are the functional paths.
- `RoomCreateService.Create()` has a live null-reference bug (`Room room = null; room.UserId = ...` — the mapper call that should construct `room` is commented out).
- Guess submission is now server-authoritative and round-complete/score-award logic is implemented for a single round per game (`RoundsPerPlayer = 1`). Round rotation (multiple drawer turns per game) and timer enforcement are still not implemented — the domain model supports them, but the service/hub logic is future work.
