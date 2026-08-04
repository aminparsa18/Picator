import type { ReactNode } from "react";

export default function PhoneFrame({ children }: { children: ReactNode }) {
  return (
    <div className="mx-auto w-[200px] rounded-[2rem] border-[8px] border-ink bg-ink shadow-ink-lg sm:w-[230px]">
      <div className="relative aspect-[412/892] overflow-hidden rounded-[2rem] bg-paper">
        <div className="absolute left-1/2 top-3 z-10 h-4 w-4 -translate-x-1/2 rounded-full bg-ink/80" />
        {children}
      </div>
    </div>
  );
}
