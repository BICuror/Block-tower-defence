using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CuroLocalization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    
    public sealed class StaticTextLocalizer : MonoBehaviour
    {
        [SerializeField] private string _key;
        private TextMeshProUGUI _textField;

        private Dictionary<string, string> _replaceSymbols;
        
        private void Awake()
        {
            _textField = GetComponent<TextMeshProUGUI>();
            LocalizationManager.OnLanguageChanged += Localize;
            Localize();
        }
        
        private void Localize()
        {
            if (string.IsNullOrEmpty(_key)) return;
            
            string localizedText = _key.Localize();
    
            _textField.text = ReplaceAllReplaceableSymbols(localizedText);
        }

        private string ReplaceAllReplaceableSymbols(string initialText)
        {
            foreach (string key in _replaceSymbols.Keys)
            {
                while (initialText.Contains(key))
                {
                    initialText = initialText.Replace(key, _replaceSymbols[key]);
                }
            }

            return initialText;
        }
        
        private void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= Localize;
        }
    }
}