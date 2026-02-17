using UnityEngine;
using TMPro;

namespace CuroLocalization
{
    [RequireComponent(typeof(TMP_Text))]
    
    public sealed class StaticTextLocalizer : MonoBehaviour
    {
        [SerializeField] private string _key;
        private TMP_Text _textField;
        
        private void Awake()
        {
            _textField = GetComponent<TMP_Text>();
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