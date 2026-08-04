import { Eye, Pencil, Trophy } from "lucide-react";
import SketchCard from "@/components/ui/SketchCard";

const STEPS = [
  {
    icon: Pencil,
    rotate: -1.5,
    title: "Draw",
    body: "One player gets a secret word and sketches it live on a shared canvas — no letters, no numbers, just shapes.",
  },
  {
    icon: Eye,
    rotate: 1,
    title: "Guess",
    body: "Everyone else races to guess from the drawing. In Teams mode it's a relay — miss it, and your teammate gets a fresh timer.",
  },
  {
    icon: Trophy,
    rotate: -0.5,
    title: "Score",
    body: "Faster guesses score more. The drawer's team even earns a small bonus when their word gets guessed correctly.",
  },
] as const;

export default function HowItWorksSection() {
  return (
    <section className="flex w-full flex-col items-center gap-12 bg-surface-alt px-6 py-24">
      <h2 className="font-heading text-4xl font-bold text-ink sm:text-5xl">How it works</h2>
      <div className="grid w-full max-w-5xl gap-10 sm:grid-cols-3">
        {STEPS.map(({ icon: Icon, rotate, title, body }) => (
          <SketchCard key={title} rotate={rotate} className="flex flex-col gap-4 p-8">
            <span className="inline-flex h-12 w-12 items-center justify-center rounded-full border-2 border-ink bg-accent-tint">
              <Icon className="h-6 w-6 text-accent-strong" strokeWidth={2} />
            </span>
            <h3 className="font-heading text-2xl font-bold text-ink">{title}</h3>
            <p className="text-base text-ink-muted">{body}</p>
          </SketchCard>
        ))}
      </div>
    </section>
  );
}
