import { BarChart3, KeyRound, Settings } from "lucide-react";

export default function HomePreview() {
  return (
    <div className="bg-dot-grid flex h-full w-full flex-col gap-7 overflow-hidden bg-paper px-5 pb-7 pt-7">
      <div className="flex items-center justify-between">
        <div className="-rotate-2 font-heading text-xl font-bold text-ink">Picator</div>
        <div className="flex items-center gap-2.5">
          <div className="flex h-9 w-9 items-center justify-center rounded-full border-2 border-ink bg-surface">
            <Settings className="h-4.5 w-4.5 text-ink" strokeWidth={2} />
          </div>
          <div className="flex h-9 w-9 items-center justify-center rounded-full border-2 border-ink bg-surface font-heading text-sm text-ink">
            P
          </div>
        </div>
      </div>

      <div className="flex flex-col gap-1">
        <div className="font-heading text-xl font-bold text-ink">Ready to play?</div>
        <div className="text-sm text-ink-muted">Pick a mode to get started.</div>
      </div>

      <div className="flex flex-col gap-4">
        <div className="shadow-ink flex -rotate-1 items-center justify-between gap-3 rounded-[20px] border-[3px] border-ink bg-accent-strong px-5 py-4">
          <div className="flex flex-col gap-1">
            <div className="font-heading text-base font-bold text-on-accent-strong">Quick Match</div>
            <div className="text-xs text-accent-tint">1v1 · jump into the next open game</div>
          </div>
          <div className="font-heading text-xl font-bold text-on-accent-strong">→</div>
        </div>

        <div className="shadow-ink flex rotate-1 items-center justify-between gap-3 rounded-[20px] border-[3px] border-ink bg-surface px-5 py-4">
          <div className="flex flex-col gap-1">
            <div className="font-heading text-base font-bold text-ink">Play with Friends</div>
            <div className="text-xs text-ink-muted">Create or join a private room</div>
          </div>
          <div className="font-heading text-xl font-bold text-ink">→</div>
        </div>
      </div>

      <div className="flex gap-3">
        <div className="flex flex-1 items-center justify-center gap-2 rounded-[14px] border-2 border-ink bg-surface-alt py-2.5 text-sm text-ink">
          <KeyRound className="h-4 w-4" strokeWidth={2} />
          Join by Code
        </div>
        <div className="flex flex-1 items-center justify-center gap-2 rounded-[14px] border-2 border-ink bg-surface-alt py-2.5 text-sm text-ink">
          <BarChart3 className="h-4 w-4" strokeWidth={2} />
          Stats
        </div>
      </div>

      <div className="flex-1" />
      <div className="self-center text-xs text-ink-muted underline decoration-dashed">How to play</div>
    </div>
  );
}
