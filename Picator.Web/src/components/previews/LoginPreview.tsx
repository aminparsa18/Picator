import { FieldPreview, GoogleGIcon } from "@/components/previews/shared";

export default function LoginPreview() {
  return (
    <div className="bg-dot-grid flex h-full w-full flex-col items-center gap-6 overflow-hidden bg-paper px-5 pb-6 pt-11">
      <div className="flex flex-col items-center gap-1.5">
        <div className="-rotate-2 font-heading text-3xl font-bold tracking-wide text-ink">Picator</div>
        <div className="h-1.5 w-16 -rotate-2 rounded-full bg-accent" />
        <div className="mt-1.5 text-sm text-ink-muted">Draw. Guess. Repeat.</div>
      </div>

      <div className="shadow-ink flex w-full max-w-[260px] -rotate-1 flex-col gap-3 rounded-[20px] border-[3px] border-ink bg-surface p-5">
        <div className="flex gap-1 rounded-xl border-2 border-ink bg-paper p-1">
          <div className="flex-1 rounded-lg bg-ink py-2 text-center text-sm text-on-ink">Sign In</div>
          <div className="flex-1 py-2 text-center text-sm text-ink-muted">Sign Up</div>
        </div>

        <FieldPreview label="Email" />
        <FieldPreview label="Password" />

        <div className="self-end text-xs text-ink-muted underline decoration-dashed">
          Forgot password?
        </div>

        <div className="shadow-ink-sm rounded-[14px] border-2 border-ink bg-accent-strong py-2.5 text-center text-sm font-bold text-on-accent-strong">
          Sign In
        </div>

        <div className="flex items-center gap-2.5">
          <div className="h-0 flex-1 border-t-2 border-dashed border-border-dashed" />
          <span className="text-xs text-ink-faint">or</span>
          <div className="h-0 flex-1 border-t-2 border-dashed border-border-dashed" />
        </div>

        <div className="shadow-ink-sm rounded-[14px] border-2 border-ink bg-surface py-2.5 text-center text-sm text-ink">
          Continue as Guest
        </div>
        <div className="shadow-ink-sm flex items-center justify-center gap-2 rounded-[14px] border-2 border-ink bg-surface py-2.5 text-center text-sm text-ink">
          {GoogleGIcon}
          Continue with Google
        </div>
      </div>
    </div>
  );
}
