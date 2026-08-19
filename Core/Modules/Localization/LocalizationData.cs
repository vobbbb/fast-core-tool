using UnityEngine;
using System;
using System.Collections.Generic;

namespace FCT.Localization
{
    public enum Language
    {
        English,
        Spanish,
        Portuguese
    }

    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        
        [Header("Standard / PC")]
        public string english;
        public string spanish;
        public string portuguese;

        [Header("Mobile Override")]
        public bool hasMobileOverride;
        public string englishMobile;
        public string spanishMobile;
        public string portugueseMobile;

        public string GetTranslation(Language lang, bool isMobile)
        {
            if (isMobile && hasMobileOverride)
            {
                switch (lang)
                {
                    case Language.Spanish: return spanishMobile;
                    case Language.Portuguese: return portugueseMobile;
                    default: return englishMobile;
                }
            }

            switch (lang)
            {
                case Language.Spanish: return spanish;
                case Language.Portuguese: return portuguese;
                default: return english;
            }
        }
    }

    [CreateAssetMenu(fileName = "LocalizationData", menuName = "FCT/Localization/Localization Data")]
    public class LocalizationData : ScriptableObject
    {
        public List<LocalizationEntry> entries = new List<LocalizationEntry>();

        public string GetText(string key, Language lang, bool isMobile)
        {
            var match = entries.Find(e => e.key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                return match.GetTranslation(lang, isMobile);
            }
            return $"MISSING_{key}";
        }
    }
}
