export default function TaglineSection() {
  return (
    <section className="bg-dot-grid flex w-full flex-col items-center justify-center gap-6 bg-paper px-6 py-24 text-center sm:py-32">
      <h1 className="font-heading text-5xl font-bold text-ink sm:text-7xl">Picator</h1>
      <p className="font-heading text-2xl text-accent sm:text-3xl">Draw. Guess. Repeat.</p>
      <p className="max-w-md text-lg text-ink-muted">
        An online multiplayer Pictionary-style game — one player draws a word on a shared
        board in real time while everyone else races to guess it.
      </p>
    </section>
  );
}
