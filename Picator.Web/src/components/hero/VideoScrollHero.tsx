"use client";

import { useCallback, useEffect, useRef, useState, useSyncExternalStore } from "react";

// Generated via (source video deleted afterward — the frames are the only copy):
// ffmpeg -i intro.mp4 -vf "fps=15,scale=1280:720:flags=lanczos" -q:v 4 intro-frames/%04d.jpg
// Served from RustFS's S3-compatible API via its fixed NodePort (see picator-api's
// 30100 gRPC NodePort in Picator.AppHost/Program.cs for the same pattern), not
// bundled under public/ anymore.
const INTRO_FRAMES_BASE_URL = "http://84.32.83.4:30101/intro-frames";
const FRAME_COUNT = 151;
const FRAME_FPS = 15;
const INTRO_SECONDS = 2;
const INTRO_END_FRAME = Math.round(INTRO_SECONDS * FRAME_FPS) + 1;
const HINT_FADE_PROGRESS = 0.05;

// Scroll-track height (must match the h-[350vh] section below) and how much of
// its pin duration, at the end, is reserved as a hold on the last frame — this
// is what gives the next section room to slide up and cover the still-pinned
// video instead of the pin releasing the instant scrubbing finishes. Keep
// HOLD_VH in sync with the -mt-[--hero-hold] reveal-overlap rule in globals.css.
const PIN_TRACK_VH = 350;
const HOLD_VH = 50;

const GRAIN_BACKGROUND_IMAGE =
  "url(\"data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='180' height='180'%3E%3Cfilter id='n'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.8' numOctaves='4' stitchTiles='stitch'/%3E%3CfeColorMatrix type='saturate' values='0'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23n)'/%3E%3C/svg%3E\")";

const REDUCED_MOTION_QUERY = "(prefers-reduced-motion: reduce)";

function frameUrl(index: number) {
  return `${INTRO_FRAMES_BASE_URL}/${String(index).padStart(4, "0")}.jpg`;
}

function subscribeToReducedMotion(callback: () => void) {
  const query = window.matchMedia(REDUCED_MOTION_QUERY);
  query.addEventListener("change", callback);
  return () => query.removeEventListener("change", callback);
}

function getReducedMotionSnapshot() {
  return window.matchMedia(REDUCED_MOTION_QUERY).matches;
}

function getReducedMotionServerSnapshot() {
  return false;
}

export default function VideoScrollHero() {
  const sectionRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const hintRef = useRef<HTMLDivElement>(null);
  const framesRef = useRef<HTMLImageElement[]>([]);
  const lastDrawnFrameRef = useRef(0);
  const [introDone, setIntroDone] = useState(false);
  const [firstFrameReady, setFirstFrameReady] = useState(false);
  const reducedMotion = useSyncExternalStore(
    subscribeToReducedMotion,
    getReducedMotionSnapshot,
    getReducedMotionServerSnapshot,
  );

  const drawFrame = useCallback((index: number) => {
    const canvas = canvasRef.current;
    const img = framesRef.current[index - 1];
    if (!canvas || !img || !img.complete || img.naturalWidth === 0) return;
    if (lastDrawnFrameRef.current === index) return;

    if (canvas.width !== img.naturalWidth || canvas.height !== img.naturalHeight) {
      canvas.width = img.naturalWidth;
      canvas.height = img.naturalHeight;
    }
    canvas.getContext("2d")?.drawImage(img, 0, 0);
    lastDrawnFrameRef.current = index;
  }, []);

  const nearestLoadedFrame = useCallback((index: number) => {
    const isLoaded = (i: number) => {
      const img = framesRef.current[i - 1];
      return !!img && img.complete && img.naturalWidth > 0;
    };
    if (isLoaded(index)) return index;
    for (let distance = 1; distance < FRAME_COUNT; distance++) {
      const back = index - distance;
      if (back >= 1 && isLoaded(back)) return back;
      const forward = index + distance;
      if (forward <= FRAME_COUNT && isLoaded(forward)) return forward;
    }
    return index;
  }, []);

  // Preload the intro frames eagerly, then the rest of the clip in small batches.
  // Runs regardless of reducedMotion: both render paths draw from this frame sequence.
  useEffect(() => {
    let cancelled = false;
    const images: HTMLImageElement[] = new Array(FRAME_COUNT);
    framesRef.current = images;

    const loadFrame = (index: number) => {
      const img = new window.Image();
      img.decoding = "async";
      img.onload = () => {
        if (cancelled || index !== 1) return;
        drawFrame(1);
        setFirstFrameReady(true);
      };
      img.src = frameUrl(index);
      images[index - 1] = img;
    };

    for (let i = 1; i <= INTRO_END_FRAME; i++) loadFrame(i);

    let cursor = INTRO_END_FRAME + 1;
    let batchTimer: ReturnType<typeof setTimeout> | null = null;
    const loadNextBatch = () => {
      if (cancelled) return;
      const end = Math.min(FRAME_COUNT, cursor + 20);
      for (let i = cursor; i <= end; i++) loadFrame(i);
      cursor = end + 1;
      if (cursor <= FRAME_COUNT) batchTimer = setTimeout(loadNextBatch, 80);
    };
    batchTimer = setTimeout(loadNextBatch, 150);

    return () => {
      cancelled = true;
      if (batchTimer) clearTimeout(batchTimer);
    };
  }, [drawFrame]);

  // Hide the browser's scrollbar chrome for as long as any part of the video
  // section is in the viewport. Scrolling itself must stay enabled — scrubbing
  // reads real scroll position (see computeScrollProgress below) — so this
  // can't just be overflow:hidden; it only suppresses the visible track/thumb.
  // Re-runs on reducedMotion because that's what swaps which <section> (and
  // thus which DOM node sectionRef points at) is actually mounted.
  useEffect(() => {
    const section = sectionRef.current;
    if (!section) return;
    const html = document.documentElement;
    const observer = new IntersectionObserver(
      ([entry]) => html.classList.toggle("hide-scrollbar", entry.isIntersecting),
      { threshold: 0 },
    );
    observer.observe(section);
    return () => {
      observer.disconnect();
      html.classList.remove("hide-scrollbar");
    };
  }, [reducedMotion]);

  // Lock page scroll for the 2s auto-play intro. Without this, a scroll during
  // that window still moves the real page (just with no visible effect on the
  // video), so the moment the intro ends the next tick reads that already-changed
  // scroll position and the video jumps straight to it instead of scrubbing.
  useEffect(() => {
    if (reducedMotion || introDone) return;
    const html = document.documentElement;
    const { body } = document;
    const previousHtmlOverflow = html.style.overflow;
    const previousBodyOverflow = body.style.overflow;
    html.style.overflow = "hidden";
    body.style.overflow = "hidden";
    return () => {
      html.style.overflow = previousHtmlOverflow;
      body.style.overflow = previousBodyOverflow;
    };
  }, [reducedMotion, introDone]);

  // Single rAF loop: auto-play frames 1 -> INTRO_END_FRAME over 2s, then hand off
  // to scroll. Scroll progress is polled from getBoundingClientRect() every frame
  // instead of driven by the native "scroll" event, because browsers coalesce/skip
  // scroll-event dispatch during momentum/fling scrolling — an event-driven update
  // would freeze mid-scroll and jump once the fling settles. A live per-frame read
  // has no such gap.
  useEffect(() => {
    if (reducedMotion) return;
    let raf = 0;
    let introStart: number | null = null;
    let introFinished = false;
    let cancelled = false;

    const computeScrollProgress = () => {
      const section = sectionRef.current;
      if (!section) return 0;
      const rect = section.getBoundingClientRect();
      const scrollableRange = rect.height - window.innerHeight;
      if (scrollableRange <= 0) return 0;
      const raw = Math.min(1, Math.max(0, -rect.top / scrollableRange));

      // Reserve the last HOLD_VH of the pin's scroll range: scrubbing finishes
      // at scrubFraction instead of 1, then holds on the last frame (still
      // pinned) for the remainder, giving the reveal-overlap room to work.
      const pinDurationVh = PIN_TRACK_VH - 100;
      const scrubFraction = (pinDurationVh - HOLD_VH) / pinDurationVh;
      return Math.min(1, raw / scrubFraction);
    };

    const tick = (time: number) => {
      if (cancelled) return;

      if (!introFinished) {
        if (introStart === null) introStart = time;
        const elapsed = (time - introStart) / 1000;

        if (elapsed >= INTRO_SECONDS) {
          introFinished = true;
          drawFrame(nearestLoadedFrame(INTRO_END_FRAME));
          setIntroDone(true);
        } else {
          const frame = Math.min(INTRO_END_FRAME, Math.floor(elapsed * FRAME_FPS) + 1);
          drawFrame(nearestLoadedFrame(frame));
        }
      } else {
        const progress = computeScrollProgress();
        const scrubbableFrames = FRAME_COUNT - INTRO_END_FRAME;
        const target = INTRO_END_FRAME + Math.round(progress * scrubbableFrames);
        drawFrame(nearestLoadedFrame(target));

        if (hintRef.current) {
          hintRef.current.style.opacity = String(1 - Math.min(1, progress / HINT_FADE_PROGRESS));
        }
      }

      raf = requestAnimationFrame(tick);
    };
    raf = requestAnimationFrame(tick);

    return () => {
      cancelled = true;
      cancelAnimationFrame(raf);
    };
  }, [reducedMotion, drawFrame, nearestLoadedFrame]);

  // Reduced motion: play the frame sequence through once at its native rate, no scroll.
  useEffect(() => {
    if (!reducedMotion) return;
    let raf = 0;
    let start: number | null = null;
    let cancelled = false;

    const tick = (time: number) => {
      if (cancelled) return;
      if (start === null) start = time;
      const elapsed = (time - start) / 1000;
      const frame = Math.min(FRAME_COUNT, Math.floor(elapsed * FRAME_FPS) + 1);
      drawFrame(nearestLoadedFrame(frame));
      if (frame < FRAME_COUNT) raf = requestAnimationFrame(tick);
    };
    raf = requestAnimationFrame(tick);

    return () => {
      cancelled = true;
      cancelAnimationFrame(raf);
    };
  }, [reducedMotion, drawFrame, nearestLoadedFrame]);

  if (reducedMotion) {
    return (
      <section ref={sectionRef} className="relative h-dvh w-full overflow-hidden bg-black">
        <canvas ref={canvasRef} className="h-full w-full object-cover" />
        <div
          aria-hidden
          className={`pointer-events-none absolute inset-0 mix-blend-overlay transition-opacity duration-300 ${
            firstFrameReady ? "opacity-100" : "opacity-0"
          }`}
          style={{ backgroundImage: GRAIN_BACKGROUND_IMAGE, backgroundRepeat: "repeat" }}
        />
      </section>
    );
  }

  return (
    <section ref={sectionRef} className="relative h-[350vh] w-full bg-black">
      <div className="sticky top-0 h-dvh w-full overflow-hidden">
        <canvas ref={canvasRef} className="h-full w-full object-cover" />
        <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-black/10" />
        <div
          aria-hidden
          className={`pointer-events-none absolute inset-0 mix-blend-overlay transition-opacity duration-300 ${
            firstFrameReady ? "opacity-100" : "opacity-0"
          }`}
          style={{ backgroundImage: GRAIN_BACKGROUND_IMAGE, backgroundRepeat: "repeat" }}
        />

        <div
          className={`pointer-events-none absolute inset-x-0 bottom-10 flex justify-center transition-opacity duration-700 ${
            introDone ? "opacity-100" : "opacity-0"
          }`}
        >
          <div
            ref={hintRef}
            className="flex flex-col items-center gap-2 text-xs font-medium uppercase tracking-[0.3em] text-white/80"
          >
            <span>Scroll</span>
            <span className="h-6 w-px animate-pulse bg-white/60" />
          </div>
        </div>
      </div>
    </section>
  );
}
