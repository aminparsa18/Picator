import { Zap, Users } from "lucide-react";
import SketchCard from "@/components/ui/SketchCard";

export default function ModesSection() {
  return (
    <section className="bg-dot-grid flex w-full flex-col items-center gap-12 bg-paper px-6 py-24">
      <h2 className="font-heading text-4xl font-bold text-ink sm:text-5xl">Two ways to play</h2>
      <div className="grid w-full max-w-4xl gap-10 sm:grid-cols-2">
        <SketchCard tone="accent" rotate={-1} shadow="lg" className="flex flex-col gap-4 p-8">
          <Zap className="h-8 w-8" strokeWidth={2} />
          <h3 className="font-heading text-2xl font-bold">Quick Match</h3>
          <p className="text-base">1v1 · jump into the next open game. Tap once, and you&apos;re paired the moment another player is ready.</p>
        </SketchCard>
        <SketchCard tone="paper" rotate={1} shadow="lg" className="flex flex-col gap-4 p-8">
          <Users className="h-8 w-8 text-accent-strong" strokeWidth={2} />
          <h3 className="font-heading text-2xl font-bold text-ink">Play with Friends</h3>
          <p className="text-base text-ink-muted">
            Create or join a private room with a code — Solo (2 players) or Teams (4 players,
            2v2 relay guessing).
          </p>
        </SketchCard>
      </div>
    </section>
  );
}
