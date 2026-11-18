using System.Collections.Generic;
using Combat.Entity.Modificators;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace Combat.IngameUI.Inspection
{
    public sealed class EntityModificatorDisplayPanel : ParsableTextContainer
    {
        [Header("Details")]
        [SerializeField] private Transform _detailsContainer;
        [SerializeField] private TextMeshProUGUI _detailsTextFieldPrefab;
        [SerializeField] private Transform _detailsSeparatorPrefab;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _modificatorHeaderText;
        [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
        
        [Header("Links")]
        [SerializeField] private UIElementFadeAnimator _entityModificatorCanvasGroup;
        [SerializeField] private CanvasGroup _negativeCanvasGroup;
        
        private List<GameObject> _createdDetailsUI = new();
        
        public void Enable(EntityModificatorConfig entityModificatorConfig)
        {
            InitializeDescription(entityModificatorConfig);
            
            _modificatorHeaderText.text = ParseTextByDefault(entityModificatorConfig.GetName());
            
            _negativeCanvasGroup.gameObject.SetActive(entityModificatorConfig.EffectType == EffectType.Negative);
            _entityModificatorCanvasGroup.Enable().Forget();
        }
        
        private void InitializeDescription(EntityModificatorConfig entityModificatorConfig)
        {
            string[] descriptions = entityModificatorConfig.GetDescription().Split('/');
            
            _modificatorDescriptionText.text = ParseTextByDefault(descriptions[0]);
            
            _createdDetailsUI.ForEach(Destroy);
    
            _detailsContainer.gameObject.SetActive(descriptions.Length > 1);
            
            for (int i = 0; i < descriptions.Length - 1; i++)
            {
                if (i > 0) _createdDetailsUI.Add(Instantiate(_detailsSeparatorPrefab, _detailsContainer).gameObject);
                
                TextMeshProUGUI detailsField = Instantiate(_detailsTextFieldPrefab, _detailsContainer);
                detailsField.text = ParseTextByDefault("startTag" + descriptions[i + 1]);
                _createdDetailsUI.Add(detailsField.gameObject);
            }
        }
    
        public void Disable()
        {
            _entityModificatorCanvasGroup.Disable().Forget();
        }
    }
}