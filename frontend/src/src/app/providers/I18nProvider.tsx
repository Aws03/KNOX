import i18n from "i18next";
import { I18nextProvider } from "react-i18next";
import { initReactI18next } from "react-i18next";
import en from "../../i18n/locales/en.json";
import ar from "../../i18n/locales/ar.json";

// Determine initial language preference (persisted in localStorage if available)
const getInitialLanguage = () => {
  try {
    if (typeof window !== "undefined") {
      return localStorage.getItem("preferred-language") ?? "ar";
    }
  } catch {
    // ignore localStorage errors
  }
  return "ar";
};

i18n.use(initReactI18next).init({
  resources: {
    en: { translation: en },
    ar: { translation: ar },
  },
  lng: getInitialLanguage(),
  fallbackLng: "ar",
});

export const I18nProvider = ({ children }: { children: React.ReactNode }) => {
  return <I18nextProvider i18n={i18n}>{children}</I18nextProvider>;
};
