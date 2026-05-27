using UnityEngine;
using TMPro;

namespace CuroLocalization
{
    [RequireComponent(typeof(TMP_Text))]
    
    public sealed class StaticTextLocalizer : MonoBehaviour
    {
        public delegate string ParseStaticText(string locKey);
        
        [SerializeField] private string _key;
        private ParseStaticText _parseStaticText;
        private TMP_Text _textField;
        
        private void Awake()
        {
            _textField = GetComponent<TMP_Text>();
            LocalizationManager.OnLanguageChanged += Localize;
            Localize();
        }

        public void SetKey(string key, ParseStaticText parseStaticText)
        {
            _key = key;
            _parseStaticText = parseStaticText;
            
            Localize();
        }
        
        private void Localize()
        {
            if (string.IsNullOrEmpty(_key)) return;

            string text = _key.Localize();
            
            if (_parseStaticText != null) text = _parseStaticText.Invoke(text);

            _textField.text = text;
        }
        
        private void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= Localize;
        }
    }
}