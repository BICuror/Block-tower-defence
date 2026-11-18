using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.Entity.Sets;
using CuroLocalization;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Combat.IngameUI.Inspection.Sets
{
    public sealed class SetDisplayPanel : ParsableTextContainer
    {
        [SerializeField] private EntitySetConfigContainer _entitySetConfigContainer;
        
        [Header("UI")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _headerTextField;
    
        [Header("Thresholds")] 
        [SerializeField] private bool _displayUpgrades;
        [SerializeField] private Transform _thresholdDisplaysParent;
        [SerializeField] private SetThresholdDisplay _thresholdThresholdDisplayPrefab;

        [Header("Animation")]
        [SerializeField] private UIElementFadeAnimator _fadeAnimator;
        private List<SetThresholdDisplay> _setThresholdDisplays = new();
        
        public void Enable(EntitySetType entitySetType, CombatEntity entity)
        {
            foreach (SetThresholdDisplay setThresholdDisplay in _setThresholdDisplays)
            {
                Destroy(setThresholdDisplay.gameObject);
            }
            
            _setThresholdDisplays.Clear();
            
            EntitySetConfig setConfig = _entitySetConfigContainer.EntitySetConfigs[entitySetType];
            
            _headerTextField.text = ParseTextByDefault(setConfig.SetNameLocKey.Localize());
            _iconImage.sprite = setConfig.SetIcon;
            
            CreateThresholdDisplays(entitySetType, entity);
            
            _fadeAnimator.Enable().Forget();
        }
    
        private void CreateThresholdDisplays(EntitySetType entitySetType, CombatEntity entity)
        {
            EntitySetConfig setConfig = _entitySetConfigContainer.EntitySetConfigs[entitySetType];
            
            int setStacks = entity.ComponentsContainer.Get<EntitySetContainer>().GetEnitySetStacks(entitySetType);
            
            foreach (EntitySetThresholdConfig entitySetThresholdConfig in setConfig.ThresholdConfigs)
            {
                SetThresholdDisplay thresholdThresholdDisplay = Instantiate(_thresholdThresholdDisplayPrefab, _thresholdDisplaysParent);
                thresholdThresholdDisplay.CopyParsersFromContainer(this);
                thresholdThresholdDisplay.SetThresholdConfig(entitySetThresholdConfig, setStacks, _displayUpgrades);
                
                _setThresholdDisplays.Add(thresholdThresholdDisplay);
            }
        }
    
        public void Disable()
        {
            _fadeAnimator.Disable().Forget();
        }
    }
}