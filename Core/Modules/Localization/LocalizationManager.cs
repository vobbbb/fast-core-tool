using UnityEngine;
using System;

namespace FCT.Localization
{
    public class LocalizationManager : FCT.Utils.FCTSingleton<LocalizationManager>
    {
        [Header("Configuration")]
        public LocalizationData data;
        public Language currentLanguage = Language.Spanish;

        public event Action OnLanguageChanged;

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance) return;
            
            DontDestroyOnLoad(gameObject);
            
            if (data == null)
            {
                data = Resources.Load<LocalizationData>("Database/LocalizationData");
            }

            LoadSettings();
        }

        public void SetLanguage(Language lang)
        {
            currentLanguage = lang;
            SaveSettings();
            OnLanguageChanged?.Invoke();
        }

        public string GetLocalText(string key)
        {
            if (data == null) return $"NO_DATA_{key}";

            bool isMobile = false;
#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#endif
            return data.GetText(key, currentLanguage, isMobile);
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt("HG_Language", (int)currentLanguage);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            currentLanguage = (Language)PlayerPrefs.GetInt("HG_Language", (int)Language.Spanish);
        }
    }
}
