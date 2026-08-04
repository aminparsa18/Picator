import { Pencil } from "lucide-react";
import { TimerBadge } from "@/components/previews/shared";

const SCRAMBLED = ["H", "O", "L", "U", "T", "G", "E", "S", "I", "H"];
const WORD_LENGTH = 10;

export default function GuessingPreview() {
  return (
    <div className="flex h-full w-full flex-col gap-3 overflow-hidden bg-paper px-4 pb-5 pt-4">
      <div className="flex items-center justify-between">
        <TimerBadge label="1:00" />
        <div className="flex flex-col items-center gap-0.5">
          <div className="text-[10px] uppercase tracking-widest text-ink-muted">Guess the word</div>
          <div className="font-heading text-base font-bold text-ink">Places</div>
        </div>
        <div className="flex items-center gap-1.5 text-xs text-ink-muted">
          <Pencil className="h-3.5 w-3.5" strokeWidth={2} />
          Jordan
        </div>
      </div>

      <div className="flex items-center justify-center gap-1.5 text-xs text-ink-muted">
        <span>Jordan is drawing</span>
        <span className="flex gap-0.5">
          <span className="h-1.5 w-1.5 rounded-full bg-ink-muted" />
          <span className="h-1.5 w-1.5 rounded-full bg-ink-muted opacity-70" />
          <span className="h-1.5 w-1.5 rounded-full bg-ink-muted opacity-40" />
        </span>
      </div>

      <div className="shadow-ink flex flex-1 items-center justify-center rounded-2xl border-[3px] border-ink bg-surface">
        <svg width="60" height="50" viewBox="0 0 100 80" aria-hidden>
          <polyline
            points="15,60 15,30 50,8 85,30 85,60"
            fill="none"
            stroke="#1A1A1A"
            strokeWidth="5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
          <line x1="8" y1="60" x2="92" y2="60" stroke="#1A1A1A" strokeWidth="5" strokeLinecap="round" />
        </svg>
      </div>

      <div className="flex flex-wrap justify-center gap-1.5">
        {Array.from({ length: WORD_LENGTH }).map((_, i) => (
          <div
            key={i}
            className="flex h-9 w-8 items-center justify-center rounded-lg border-2 border-ink bg-surface font-heading text-lg font-bold text-ink"
          />
        ))}
      </div>

      <div className="h-px bg-border-hairline" />

      <div className="flex flex-wrap justify-center gap-1.5">
        {SCRAMBLED.map((letter, i) => (
          <div
            key={i}
            className="shadow-ink-sm flex h-9 w-8 items-center justify-center rounded-lg border-2 border-ink bg-surface-alt font-heading text-lg font-bold text-ink"
          >
            {letter}
          </div>
        ))}
      </div>

      <div className="flex-1" />

      <div className="flex gap-2.5">
        <div className="flex-1 rounded-[14px] border-2 border-ink bg-surface py-2.5 text-center text-sm text-ink">
          Clear
        </div>
        <div className="shadow-ink-sm flex-[2] rounded-[14px] border-2 border-ink bg-accent-tint py-2.5 text-center text-sm text-ink-muted">
          Submit Guess
        </div>
      </div>
    </div>
  );
}
