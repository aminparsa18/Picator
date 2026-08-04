export const GoogleGIcon = (
  <svg width="16" height="16" viewBox="0 0 24 24" aria-hidden>
    <path
      fill="#1A1A1A"
      d="M23.52 12.27c0-.85-.08-1.67-.22-2.45H12v4.63h6.48a5.54 5.54 0 0 1-2.4 3.63v3.02h3.88c2.27-2.09 3.56-5.17 3.56-8.83z"
    />
    <path
      fill="#5C5A54"
      d="M12 24c3.24 0 5.95-1.07 7.96-2.9l-3.88-3.02c-1.08.73-2.46 1.16-4.08 1.16-3.14 0-5.8-2.12-6.75-4.96H1.24v3.11C3.26 21.3 7.3 24 12 24z"
    />
    <path
      fill="#A8A59C"
      d="M5.25 14.28A7.14 7.14 0 0 1 4.86 12c0-.79.14-1.56.39-2.28V6.61H1.24A11.93 11.93 0 0 0 0 12c0 1.93.46 3.76 1.24 5.39l4.01-3.11z"
    />
    <path
      fill="#1A1A1A"
      d="M12 4.75c1.77 0 3.35.61 4.6 1.8l3.44-3.44C17.94 1.19 15.24 0 12 0 7.3 0 3.26 2.7 1.24 6.61l4.01 3.11C6.2 6.87 8.86 4.75 12 4.75z"
    />
  </svg>
);

export function FieldPreview({ label, mono }: { label: string; mono?: boolean }) {
  return (
    <div
      className={`w-full rounded-[10px] border-2 border-ink bg-surface px-3.5 py-2.5 text-sm text-ink-faint ${
        mono ? "text-center font-mono tracking-[4px]" : ""
      }`}
    >
      {label}
    </div>
  );
}

export function TimerBadge({ label }: { label: string }) {
  return (
    <div className="rounded-[10px] border-2 border-ink bg-surface px-2.5 py-1 font-mono text-sm text-ink">
      {label}
    </div>
  );
}
