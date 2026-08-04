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
      <TaglineSection />
      <HowItWorksSection />
      <ModesSection />
      <FeaturesSection />
      <AppPreviewSection />
      <DownloadCtaSection />
      <SiteFooter />
    </>
  );
}
