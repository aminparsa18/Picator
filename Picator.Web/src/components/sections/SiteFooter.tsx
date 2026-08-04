export default function SiteFooter() {
  return (
    <footer className="flex w-full flex-col items-center gap-2 border-t-2 border-border-hairline bg-paper px-6 py-8 text-center">
      <span className="font-heading text-lg font-bold text-ink">Picator</span>
      <p className="text-xs text-ink-faint">© {new Date().getFullYear()} Picator. All rights reserved.</p>
    </footer>
  );
}
