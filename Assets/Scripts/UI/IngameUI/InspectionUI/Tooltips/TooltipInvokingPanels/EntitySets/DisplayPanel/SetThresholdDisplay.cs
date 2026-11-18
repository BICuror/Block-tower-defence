using Combat.Entity.Sets;
using CuroLocalization;
using UnityEngine;
using TMPro;

namespace Combat.IngameUI.Inspection.Sets
{
    public sealed class SetThresholdDisplay : ParsableTextContainer
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _effectDescriptionTextField;
        [SerializeField] private TextMeshProUGUI _thresholdStacksTextField;
    
        [Header("StateView")]
        [SerializeField] private Color _upgradeColor;
        [SerializeField] private Color _enabledColor;
        [SerializeField] private Color _disabledColor;
        
        private EntitySetThresholdConfig _entitySetThresholdConfig;
        
        public void SetThresholdConfig(EntitySetThresholdConfig config, int currentLevel, bool displaysUpgrades)
        {
            _entitySetThresholdConfig = config;
            _thresholdStacksTextField.text = config.LevelThreshold.ToString();
            _effectDescriptionTextField.text = ParseTextByDefault(config.EntityModificator.ParseByConfig(_entitySetThresholdConfig.ThresholdDescriptionEffectLocKey.Localize()));
            
            Color color = currentLevel >= config.LevelThreshold ? _enabledColor : _disabledColor;
    
            if (displaysUpgrades && currentLevel + 1 == config.LevelThreshold) color = _upgradeColor;
            
            _effectDescriptionTextField.color = color;
            _thresholdStacksTextField.color = color;
        }
    }
}