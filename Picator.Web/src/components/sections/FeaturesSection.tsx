import { BarChart3, Clock, KeyRound, MonitorSmartphone, Pencil, Users } from "lucide-react";
import SketchCard from "@/components/ui/SketchCard";

const FEATURES = [
  { icon: Pencil, title: "Real-time drawing", body: "Every stroke streams live to the room as it's drawn — no refreshing, no lag." },
  { icon: Clock, title: "Live timer & scoring", body: "A ticking round timer and running score, always visible." },
  { icon: KeyRound, title: "Private rooms", body: "Create a room, share the code, and play with exactly who you invite." },
  { icon: Users, title: "Team relay guessing", body: "2v2 Teams mode hands the guess off between teammates under a fresh timer." },
  { icon: MonitorSmartphone, title: "Phone & desktop", body: "One MAUI app, built to run on mobile and desktop alike." },
  { icon: BarChart3, title: "Lifetime score", body: "Every game you finish rolls into your all-time total." },
] as const;

export default function FeaturesSection() {
  return (
    <section className="flex w-full flex-col items-center gap-12 bg-surface-alt px-6 py-24">
      <h2 className="font-heading text-4xl font-bold text-ink sm:text-5xl">What you get</h2>
      <div className="grid w-full max-w-5xl gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {FEATURES.map(({ icon: Icon, title, body }) => (
          <SketchCard key={title} shadow="sm" className="flex flex-col gap-3 p-6">
            <Icon className="h-6 w-6 text-ink-muted" strokeWidth={2} />
            <h3 className="font-heading text-lg font-bold text-ink">{title}</h3>
            <p className="text-sm text-ink-muted">{body}</p>
          </SketchCard>
        ))}
      </div>
    </section>
  );
}
