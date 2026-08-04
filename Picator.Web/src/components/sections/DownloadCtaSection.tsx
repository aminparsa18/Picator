import { Apple, PlayCircle } from "lucide-react";
import SketchCard from "@/components/ui/SketchCard";

export default function DownloadCtaSection() {
  return (
    <section className="bg-dot-grid flex w-full flex-col items-center bg-paper px-6 py-24">
      <SketchCard
        tone="accent"
        shadow="lg"
        border={3}
        rotate={-1}
        radius="rounded-[20px]"
        className="flex w-full max-w-2xl flex-col items-center gap-6 p-10 text-center sm:p-14"
      >
        <h2 className="font-heading text-4xl font-bold sm:text-5xl">Picator</h2>
        <p className="font-heading text-xl sm:text-2xl">Draw. Guess. Repeat.</p>
        <div className="mt-4 flex flex-col gap-4 sm:flex-row">
          <a
            href="#"
            className="shadow-ink-sm inline-flex items-center justify-center gap-2 rounded-xl border-2 border-ink bg-on-accent px-6 py-3 font-heading text-lg font-bold text-ink transition-transform hover:-translate-y-0.5"
          >
            <Apple className="h-5 w-5" strokeWidth={2} />
            View on the App Store
          </a>
          <a
            href="#"
            className="shadow-ink-sm inline-flex items-center justify-center gap-2 rounded-xl border-2 border-ink bg-on-accent px-6 py-3 font-heading text-lg font-bold text-ink transition-transform hover:-translate-y-0.5"
          >
            <PlayCircle className="h-5 w-5" strokeWidth={2} />
            Get it on Google Play
          </a>
        </div>
      </SketchCard>
    </section>
  );
}
