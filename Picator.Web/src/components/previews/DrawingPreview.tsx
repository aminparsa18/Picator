import { Send, Trash2, Undo2 } from "lucide-react";
import { TimerBadge } from "@/components/previews/shared";

const PALETTE = ["#1A1A1A", "#C7431F", "#E8532E", "#2F9E44", "#1D5FAD", "#F2B705", "#8E44AD", "#FDFCF8"];
const BRUSH_SIZES = [6, 10, 14];

export default function DrawingPreview() {
  return (
    <div className="flex h-full w-full flex-col gap-3 overflow-hidden bg-paper px-4 pb-5 pt-4">
      <div className="flex items-center justify-between">
        <TimerBadge label="1:00" />
        <div className="flex flex-col items-center gap-0.5">
          <div className="text-[10px] uppercase tracking-widest text-ink-muted">Draw</div>
          <div className="font-heading text-lg font-bold text-ink">Lighthouse</div>
        </div>
        <div className="flex items-center gap-1.5 text-xs text-ink-muted">
          <Send className="h-3.5 w-3.5" strokeWidth={2} />
          Jordan
        </div>
      </div>

      <div className="shadow-ink flex-1 rounded-2xl border-[3px] border-ink bg-surface" />

      <div className="flex items-center gap-2">
        {PALETTE.map((color, i) => (
          <div
            key={color}
            className="h-5 w-5 rounded-full"
            style={{
              background: color,
              border: i === 0 ? "3px solid var(--ink)" : "2px solid rgb(26 26 26 / 25%)",
            }}
          />
        ))}
        <div className="flex-1" />
        <div className="flex h-8 w-8 items-center justify-center rounded-[10px] border-2 border-ink bg-surface">
          <Undo2 className="h-3.5 w-3.5 text-ink" strokeWidth={2} />
        </div>
        <div className="flex h-8 w-8 items-center justify-center rounded-[10px] border-2 border-ink bg-surface">
          <Trash2 className="h-3.5 w-3.5 text-ink" strokeWidth={2} />
        </div>
      </div>

      <div className="flex items-center gap-2.5">
        <span className="text-xs text-ink-muted">Brush</span>
        {BRUSH_SIZES.map((size, i) => (
          <div
            key={size}
            className={`flex h-7 w-7 items-center justify-center rounded-full ${
              i === 1 ? "border-2 border-ink bg-paper" : ""
            }`}
          >
            <span className="rounded-full bg-ink" style={{ width: size, height: size }} />
          </div>
        ))}
      </div>
    </div>
  );
}
