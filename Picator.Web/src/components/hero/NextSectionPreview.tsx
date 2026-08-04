export default function NextSectionPreview() {
  return (
    <section className="flex min-h-dvh w-full flex-col items-center justify-center gap-4 bg-zinc-50 px-6 text-center dark:bg-black">
      <p className="text-xs font-medium uppercase tracking-[0.3em] text-zinc-500 dark:text-zinc-400">
        Section 2 &mdash; placeholder
      </p>
      <h2 className="max-w-xl text-3xl font-semibold tracking-tight text-zinc-950 sm:text-4xl dark:text-zinc-50">
        Scroll landed here once the intro video finished scrubbing.
      </h2>
      <p className="max-w-md text-base text-zinc-600 dark:text-zinc-400">
        Swap this section out for real content once the hero handoff feels right.
      </p>
    </section>
  );
}
