# Picator — Design System (Color + Style Brief)

Handoff brief for generating UI. Covers visual direction, typography, and the full color token set. For gameplay/domain context, see [`README.md`](README.md) and [`GAME_RULES.md`](GAME_RULES.md).

## Product

Picator is an online multiplayer Pictionary-style game: one player draws a word on a shared board in real time while others guess. Client is a .NET MAUI app (phone + desktop). Modes: **Quick Match** (1v1), **Solo room** (1v1), **Teams room** (2v2, relay guessing).

## Design direction

**Style: Sketch / Hand-Drawn Paper.** Warm paper background, near-black ink, one marker-red/orange accent — the UI should look like it's drawn on paper, echoing the game's actual mechanic (players drawing by hand). This isn't decorative theming; it's chosen because the interface visually reinforces what the product does.

Style cues to carry into components:
- **Borders over shadows-as-depth:** solid 2–4px ink borders define cards/buttons/inputs rather than soft drop shadows. A hard, small offset shadow (not blurred) is fine for a "pressed paper" feel.
- **Rounded but not soft:** 8–24px corner radii depending on element size (chips small, cards larger). Avoid fully soft/blurred neumorphic or glassmorphic treatments — they fight the paper metaphor.
- **Flat color, no gradients.** Marker/ink colors are flat fills.
- **Light hand-drawn imperfection is welcome** (slight rotation on cards, wobbly corners, dashed separators) but should stay subtle — this is a live scoring/timer game, not a scrapbook; legibility and speed of reading (timer, score, whose turn) always win over decoration.
- **Monospace for data:** timers, scores, room codes use a monospace face so digits don't shift width as they tick.

## Typography

| Role | Font | Notes |
|---|---|---|
| Headings / wordmark | Kalam (Bold) | Felt-tip marker weight for titles, round headers |
| Body / UI labels / buttons | Patrick Hand | Legible handwriting face, used everywhere else |
| Data (timer, score, room code, chat meta) | Space Mono | Tabular figures so numbers don't jitter |

Google Fonts import:
```
https://fonts.googleapis.com/css2?family=Kalam:wght@400;700&family=Patrick+Hand&family=Space+Mono&display=swap
```

## Color system

Source of truth: [`Picator.GameV2/Resources/Styles/Colors.xaml`](Picator.GameV2/Resources/Styles/Colors.xaml). All pairs below are contrast-checked against WCAG AA (4.5:1 body text / 3:1 large text or UI).

### Neutrals — paper & ink

| Token | Light | Dark | Usage |
|---|---|---|---|
| Background | `#F0EEE9` | `#1B1A17` | App/page background ("paper") |
| Background (sunken) | `#E4E1D8` | `#2C2A26` | Empty-canvas / placeholder pattern fill |
| Surface | `#FDFCF8` | `#242220` | Cards, sheets, inputs |
| Surface Alt | `#F5F3EE` | `#2C2A26` | Secondary panel, inactive tab |
| Ink (text primary / strong borders) | `#1A1A1A` | `#F0EEE9` | Primary text, 2–4px borders, filled default-button bg |
| Ink Muted (text secondary) | `#5C5A54` | `#A6A39B` | Secondary text, labels |
| Ink Faint (placeholder/disabled) | `#A8A59C` | `#6E6B63` | Placeholder text, disabled content |
| On-Ink (text/icon on ink-filled surface) | `#FDFCF8` | `#1A1A1A` | Label on a default filled button |
| Border — hairline | `#1A1A1A` @14% | `#FDFCF8` @14% | Thin dividers |
| Border — dashed | `#1A1A1A` @20% | `#FDFCF8` @20% | Dashed separators (round feed, section breaks) |
| Overlay / scrim | `#1A1A1A` @50% | `#000000` @60% | Modal / sheet backdrop |

### Brand — primary action & accent

| Token | Light | Dark | Usage |
|---|---|---|---|
| Accent | `#E8532E` | `#FF6B45` | **Decorative / large use only:** active tab indicator, timer arc, icon fills, borders ≥3px, large/bold chip text |
| On Accent | `#FFFFFF` (large/bold text only) | `#1B1A17` | Text on Accent — light-mode Accent only clears AA contrast at large/bold sizes |
| Accent Strong | `#C7431F` | *(reuse Accent — already AA at all sizes on dark bg)* | Solid buttons / small-text badges where the fill needs body-text contrast |
| On Accent Strong | `#FFFFFF` | — | |
| Accent Hover/Press | `#B23A1A` | — | |
| Accent Tint | `#FBE0D6` | `#FF6B45` @20% | Subtle chip/badge fill, pair with Ink text |

**Rule:** Accent is the single brand/CTA color — the primary "call to action" and "this is active/happening now" signal (Start Matching, active tab, timer ring, "your turn" chip). It is never reused for team identity, so a player never confuses "the app wants my attention" with "that's my opponent's color."

### Teams (2v2 mode only)

| Token | Light | Dark | Usage |
|---|---|---|---|
| Team A | `#3161E0` | `#7FA0F5` | Team A identity — avatars, borders, score bar |
| On Team A | `#FFFFFF` | `#1B1A17` | |
| Team A Tint | `#E1E9FB` | — | Subtle background fill (lobby column, score row) |
| Team B | `#8347E5` | `#B79AF2` | Team B identity |
| On Team B | `#FFFFFF` | `#1B1A17` | |
| Team B Tint | `#EAE1FB` | — | |

**Rule:** never rely on color alone to show team — always pair with a label ("Team A"/"Team B") or letter badge, for colorblind players.

### Status feedback

| Token | Light | Dark | Usage |
|---|---|---|---|
| Success | `#2F9E44` | `#5CC97C` | Correct guess, win state — decorative/large use |
| Success Strong | `#1F7A34` | *(reuse Success)* | Small-text-on-fill (badges) |
| On Success | `#FFFFFF` | `#1B1A17` | |
| Warning | `#D98C0A` | `#F0AC3D` | Low-time caution state |
| On Warning | `#1A1A1A` (always) | `#1B1A17` | White fails contrast on this amber in both modes — always pair with ink text |
| Error | `#C1272D` | `#E8615F` | Destructive actions (leave room, kick, delete) — **not** used for in-round wrong-guess feedback, which stays neutral ink |
| On Error | `#FFFFFF` | `#1B1A17` | |

### Timer / urgency states

- Normal countdown → **Accent** (matches the marker-red ring).
- Critical / last few seconds → **Error**, pulsing. No extra hue introduced — reuses the destructive red, which already reads as "urgent."

## Usage rules (do / don't)

- **Do** treat Accent as scarce — one primary CTA per screen, timer ring, active-tab indicator. If everything is accent-colored, nothing reads as "the important thing."
- **Do** use Accent Strong (not Accent) any time white text sits on the fill at body/label size — Accent alone only passes contrast at large/bold sizes.
- **Do** keep Team A/B colors out of any non-team UI (buttons, nav, status) — they mean "team," nothing else.
- **Don't** put white text on Warning in either theme — always ink/dark text.
- **Don't** invert dark mode naively. Background flips to a warm near-black (`#1B1A17`), not pure black; team/status colors get lightened pastel variants that pair with **dark ink text**, not white (verified — white fails contrast on all of them).
- **Don't** introduce new hues casually — extend from AccentTint/TeamTint/StatusTint families before adding a new color.

## Screens this needs to cover

For context on what's being designed (from existing low-fi wireframes — layout/composition is open, not prescribed):

1. Home / game menu — Quick Match vs Play with Friends
2. Room creation — Solo vs Teams format picker
3. Room lobby — waiting for players, share room code
4. Drawing screen — drawer's view (canvas, word prompt, timer, tools, undo/clear)
5. Guessing screen — solo guesser, and Teams relay (whose turn to guess, hand-off)
6. Round-end reveal — word reveal, points awarded, next-drawer preview
7. Game-end results — Solo ranking / Teams score comparison + MVP, rematch

## Implementation note

Colors already exist as MAUI `Color`/`SolidColorBrush` resources in `Picator.GameV2/Resources/Styles/Colors.xaml`, added additively (existing legacy template keys like `Primary`, `Gray300` are untouched so current screens are unaffected). When building new screens, prefer the semantic keys in this doc over the legacy ones.
