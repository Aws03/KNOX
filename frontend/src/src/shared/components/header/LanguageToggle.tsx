import { useState, useEffect } from "react";
import i18n from "i18next";
import { Button } from "../../ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../../ui/dropdown-menu";
import { Globe, Check } from "lucide-react";

const LanguageToggle = () => {
  const [language, setLanguage] = useState(() =>
    typeof window !== "undefined"
      ? localStorage.getItem("preferred-language") ?? "en"
      : "en"
  );

  const languages = [
    { code: "en", name: "English", nativeName: "English" },
    { code: "ar", name: "Arabic", nativeName: "العربية" },
  ];

  const handleLanguageChange = (langCode: string) => {
    // Only update React state here; side-effects run in useEffect
    setLanguage(langCode);
  };

  // Sync DOM, localStorage and dispatch events when language changes
  useEffect(() => {
    // Update document direction for RTL support
    document.documentElement.dir = language === "ar" ? "rtl" : "ltr";
    document.documentElement.lang = language;

    // Store preference in localStorage
    try {
      localStorage.setItem("preferred-language", language);
    } catch {
      // ignore if localStorage is not available
    }

    // Tell i18next to change language
    try {
      i18n.changeLanguage(language);
    } catch {
      // ignore if i18n is not initialized yet
    }

    // Dispatch custom event for app-wide language change
    window.dispatchEvent(
      new CustomEvent("languageChange", { detail: { language } })
    );
  }, [language]);

  const currentLanguage = languages.find((lang) => lang.code === language);

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="sm" className="group gap-2 h-9 px-3">
          <Globe className="h-4 w-4 transition-transform group-hover:scale-110" />
          <span className="hidden sm:inline-flex font-medium">
            {currentLanguage?.nativeName}
          </span>
          <span className="sr-only">Toggle language</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-40">
        {languages.map((lang) => (
          <DropdownMenuItem
            key={lang.code}
            onClick={() => handleLanguageChange(lang.code)}
            className="flex items-center justify-between cursor-pointer"
          >
            <span className="font-medium">{lang.nativeName}</span>
            {language === lang.code && (
              <Check className="h-4 w-4 text-primary" />
            )}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default LanguageToggle;
