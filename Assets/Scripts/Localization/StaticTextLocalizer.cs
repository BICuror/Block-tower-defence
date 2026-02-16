using UnityEngine;
using TMPro;

namespace CuroLocalization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    
    public sealed class StaticTextLocalizer : MonoBehaviour
    {
        [SerializeField] private string _key;
        private TextMeshProUGUI _textField;
        
        private void Awake()
        {
            _textField = GetComponent<TextMeshProUGUI>();
            LocalizationManager.OnLanguageChanged += Localize;
            Localize();
        }
        
        private void Localize()
        {
            if (string.IsNullOrEmpty(_key)) return;
    
            _textField.text = _key.Localize();
        }
        
        private void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= Localize;
        }
    }
}