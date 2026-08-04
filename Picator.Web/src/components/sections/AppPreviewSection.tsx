import ScreenshotCarousel from "@/components/ui/ScreenshotCarousel";
import LoginPreview from "@/components/previews/LoginPreview";
import HomePreview from "@/components/previews/HomePreview";
import DrawingPreview from "@/components/previews/DrawingPreview";
import GuessingPreview from "@/components/previews/GuessingPreview";
import ProfilePreview from "@/components/previews/ProfilePreview";
import RoundIntroPreview from "@/components/previews/RoundIntroPreview";

const SCREENS = [
  { label: "Login", node: <LoginPreview /> },
  { label: "Home", node: <HomePreview /> },
  { label: "Round Intro", node: <RoundIntroPreview /> },
  { label: "Drawing", node: <DrawingPreview /> },
  { label: "Guessing", node: <GuessingPreview /> },
  { label: "Profile", node: <ProfilePreview /> },
];

export default function AppPreviewSection() {
  return (
    <section className="bg-dot-grid flex w-full flex-col items-center gap-8 bg-paper px-6 py-16">
      <div className="flex flex-col items-center gap-2 text-center">
        <h2 className="font-heading text-3xl font-bold text-ink sm:text-4xl">See it in action</h2>
        <p className="max-w-md text-sm text-ink-muted sm:text-base">
          Sketchbook style, right down to the sign-in screen.
        </p>
      </div>

      <ScreenshotCarousel items={SCREENS} />
    </section>
  );
}
