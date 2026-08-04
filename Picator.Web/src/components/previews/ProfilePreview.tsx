import { ArrowLeft, LogOut, Pencil } from "lucide-react";
import { FieldPreview } from "@/components/previews/shared";

export default function ProfilePreview() {
  return (
    <div className="bg-dot-grid flex h-full w-full flex-col gap-6 overflow-hidden bg-paper px-5 pb-7 pt-6">
      <div className="flex items-center gap-3">
        <div className="flex h-9 w-9 items-center justify-center rounded-full border-2 border-ink bg-surface">
          <ArrowLeft className="h-4 w-4 text-ink" strokeWidth={2} />
        </div>
        <div className="font-heading text-lg font-bold text-ink">Profile</div>
        <div className="flex-1" />
        <div className="flex h-9 w-9 items-center justify-center rounded-full border-2 border-ink bg-surface">
          <LogOut className="h-4 w-4 text-error" strokeWidth={2} />
        </div>
      </div>

      <div className="flex flex-col items-center gap-2">
        <div className="relative">
          <div className="shadow-ink-sm flex h-20 w-20 items-center justify-center rounded-full border-[3px] border-ink bg-accent-strong font-heading text-2xl font-bold text-on-accent-strong">
            P
          </div>
          <div className="absolute -bottom-1 -right-1 flex h-7 w-7 items-center justify-center rounded-full border-2 border-ink bg-surface">
            <Pencil className="h-3 w-3 text-ink" strokeWidth={2} />
          </div>
        </div>
        <div className="text-xs text-ink-muted">Tap the pencil to change your avatar</div>
      </div>

      <div className="shadow-ink flex flex-col gap-2.5 rounded-[20px] border-[3px] border-ink bg-surface p-4">
        <div className="font-heading text-sm font-bold text-ink">Display Name</div>
        <FieldPreview label="Player One" />
        <div className="rounded-xl border-2 border-ink bg-accent-strong py-2 text-center text-sm text-on-accent-strong">
          Save Name
        </div>
      </div>

      <div className="shadow-ink flex flex-col gap-2.5 rounded-[20px] border-[3px] border-ink bg-surface p-4">
        <div className="font-heading text-sm font-bold text-ink">Change Password</div>
        <FieldPreview label="Current password" />
        <FieldPreview label="New password" />
        <FieldPreview label="Confirm new password" />
        <div className="rounded-xl border-2 border-ink bg-accent-strong py-2 text-center text-sm text-on-accent-strong">
          Update Password
        </div>
      </div>
    </div>
  );
}
