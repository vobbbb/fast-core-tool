using UnityEngine;
using TMPro;

namespace FCT.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizeText : MonoBehaviour
    {
        public string localizationKey;
        private TextMeshProUGUI _textElement;

        void Awake()
        {
            _textElement = GetComponent<TextMeshProUGUI>();
        }

        void OnEnable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += RefreshText;
                RefreshText();
            }
            else
            {
                // If manager is not ready, wait and try again
                StartCoroutine(WaitAndSubscribe());
            }
        }

        private System.Collections.IEnumerator WaitAndSubscribe()
        {
            while (LocalizationManager.Instance == null)
            {
                yield return null;
            }
            LocalizationManager.Instance.OnLanguageChanged += RefreshText;
            RefreshText();
        }

        void Start()
        {
            RefreshText();
        }

        void OnDisable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= RefreshText;
            }
        }

        [ContextMenu("Refresh Text")]
        public void RefreshText()
        {
            if (LocalizationManager.Instance == null || string.IsNullOrEmpty(localizationKey)) return;
            
            string localized = LocalizationManager.Instance.GetLocalText(localizationKey);
            if (!string.IsNullOrEmpty(localized))
            {
                _textElement.text = localized;
            }
        }
    }
}
