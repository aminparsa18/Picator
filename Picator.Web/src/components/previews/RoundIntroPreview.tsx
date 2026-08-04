import { Ban, Clock, Pencil } from "lucide-react";

const TIPS = [
  { icon: Pencil, text: "Shapes and outlines first — details later if there's time." },
  { icon: Ban, text: "No letters, numbers, or words in your drawing." },
  { icon: Clock, text: "You'll have limited time once the round starts — work fast." },
];

export default function RoundIntroPreview() {
  return (
    <div className="bg-dot-grid flex h-full w-full flex-col items-center gap-5 overflow-hidden bg-paper px-5 pb-7 pt-6">
      <div className="rounded-[10px] border-2 border-ink bg-surface px-3.5 py-1.5 text-xs text-ink">
        Preview as Guesser
      </div>

      <div className="flex flex-col items-center gap-1.5">
        <div
          className="flex h-[84px] w-[84px] items-center justify-center rounded-full p-1.5"
          style={{ background: "conic-gradient(#E8532E 288deg, #E4E1D8 0deg)" }}
        >
          <div className="flex h-full w-full items-center justify-center rounded-full border-2 border-ink bg-paper font-mono text-xl text-ink">
            4
          </div>
        </div>
        <div className="text-xs text-ink-muted">Round starts in</div>
      </div>

      <div className="rounded-full border-2 border-ink bg-accent-strong px-4 py-1.5 text-xs text-on-accent-strong">
        You&apos;re the Drawer
      </div>

      <div className="shadow-ink flex w-full -rotate-1 flex-col items-center gap-1.5 rounded-[20px] border-[3px] border-ink bg-surface px-5 py-5">
        <div className="text-[10px] uppercase tracking-widest text-ink-muted">Your word</div>
        <div className="font-heading text-2xl font-bold text-ink">Lighthouse</div>
        <div className="text-xs text-ink-faint">Category: Places</div>
      </div>

      <div className="flex w-full flex-col gap-2.5 rounded-2xl border-2 border-ink bg-surface-alt p-4">
        <div className="font-heading text-sm font-bold text-ink">Drawing tips</div>
        {TIPS.map(({ icon: Icon, text }) => (
          <div key={text} className="flex items-start gap-2.5 text-xs leading-relaxed text-ink-muted">
            <Icon className="mt-0.5 h-3.5 w-3.5 flex-shrink-0" strokeWidth={2} />
            <span>{text}</span>
          </div>
        ))}
      </div>

      <div className="flex-1" />
      <div className="text-center text-[11px] text-ink-faint">
        Get ready — the timer starts the round automatically.
      </div>
    </div>
  );
}
