import type { ReactNode } from "react";

type SketchCardTone = "paper" | "surface" | "accent";
type SketchCardShadow = "sm" | "md" | "lg";

const TONE_CLASSES: Record<SketchCardTone, string> = {
  paper: "bg-paper text-ink",
  surface: "bg-surface text-ink",
  accent: "bg-accent text-on-accent",
};

const SHADOW_CLASSES: Record<SketchCardShadow, string> = {
  sm: "shadow-ink-sm",
  md: "shadow-ink",
  lg: "shadow-ink-lg",
};

interface SketchCardProps {
  children: ReactNode;
  className?: string;
  tone?: SketchCardTone;
  shadow?: SketchCardShadow;
  border?: 2 | 3;
  /** Slight hand-drawn tilt, in degrees. Keep small (±0.5–2°). */
  rotate?: number;
  radius?: string;
}

export default function SketchCard({
  children,
  className = "",
  tone = "surface",
  shadow = "md",
  border = 2,
  rotate = 0,
  radius = "rounded-2xl",
}: SketchCardProps) {
  return (
    <div
      className={`${TONE_CLASSES[tone]} ${SHADOW_CLASSES[shadow]} ${
        border === 3 ? "border-[3px]" : "border-2"
      } border-ink ${radius} ${className}`}
      style={rotate ? { transform: `rotate(${rotate}deg)` } : undefined}
    >
      {children}
    </div>
  );
}
