import VideoScrollHero from "@/components/hero/VideoScrollHero";
import TaglineSection from "@/components/sections/TaglineSection";
import HowItWorksSection from "@/components/sections/HowItWorksSection";
import ModesSection from "@/components/sections/ModesSection";
import FeaturesSection from "@/components/sections/FeaturesSection";
import AppPreviewSection from "@/components/sections/AppPreviewSection";
import DownloadCtaSection from "@/components/sections/DownloadCtaSection";
import SiteFooter from "@/components/sections/SiteFooter";

export default function Home() {
  return (
    <>
      <VideoScrollHero />
      {/* Slides up over the still-pinned last frame of the hero instead of
          only appearing once the pin releases — see HOLD_VH in VideoScrollHero
          and .hero-reveal-overlap in globals.css. */}
      <div className="hero-reveal-overlap relative z-10">
        <TaglineSection />
        <HowItWorksSection />
        <ModesSection />
        <FeaturesSection />
        <AppPreviewSection />
        <DownloadCtaSection />
        <SiteFooter />
      </div>
    </>
  );
}
