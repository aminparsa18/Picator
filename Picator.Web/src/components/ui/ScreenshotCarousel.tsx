"use client";

import { useState, type ReactNode } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import PhoneFrame from "@/components/ui/PhoneFrame";

interface CarouselItem {
  label: string;
  node: ReactNode;
}

export default function ScreenshotCarousel({ items }: { items: CarouselItem[] }) {
  const [index, setIndex] = useState(0);
  const active = items[index];

  const goPrev = () => setIndex((i) => (i - 1 + items.length) % items.length);
  const goNext = () => setIndex((i) => (i + 1) % items.length);

  return (
    <div className="flex flex-col items-center gap-5">
      <PhoneFrame>{active.node}</PhoneFrame>

      <div className="flex items-center gap-4">
        <button
          type="button"
          onClick={goPrev}
          aria-label="Previous screen"
          className="flex h-10 w-10 cursor-pointer items-center justify-center rounded-full border-2 border-ink bg-surface text-ink transition-transform hover:-translate-x-0.5"
        >
          <ChevronLeft className="h-5 w-5" strokeWidth={2} />
        </button>

        <div className="flex items-center gap-2">
          {items.map((item, i) => (
            <button
              key={item.label}
              type="button"
              onClick={() => setIndex(i)}
              aria-label={`Show ${item.label}`}
              aria-current={i === index}
              className={`h-2.5 w-2.5 cursor-pointer rounded-full border-2 border-ink ${
                i === index ? "bg-ink" : "bg-transparent"
              }`}
            />
          ))}
        </div>

        <button
          type="button"
          onClick={goNext}
          aria-label="Next screen"
          className="flex h-10 w-10 cursor-pointer items-center justify-center rounded-full border-2 border-ink bg-surface text-ink transition-transform hover:translate-x-0.5"
        >
          <ChevronRight className="h-5 w-5" strokeWidth={2} />
        </button>
      </div>
    </div>
  );
}
