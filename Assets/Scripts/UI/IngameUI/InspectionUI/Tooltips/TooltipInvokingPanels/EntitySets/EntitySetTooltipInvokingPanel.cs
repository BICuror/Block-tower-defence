using Combat.Entity.Sets;
using NaughtyAttributes;
using CuroLocalization;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Combat.IngameUI.Inspection.Tooltips
{
    public sealed class EntitySetTooltipInvokingPanel : TooltipInvokingPanel
    {
        [SerializeField] private EntitySetConfigContainer _entitySetConfigContainer;

        [Header("UI")] 
        [SerializeField] private Image _iconImage;

        [Header("Stacks")] 
        [SerializeField] private bool _displayCurrentSetStacks = true;
        [SerializeField, ShowIf("_displayCurrentSetStacks")] private TextMeshProUGUI _stackCountTextField;

        private TooltipParseTagDataContainer _tagDataContainer;
        private EntitySetConfig _entitySetConfig;
        private EntitySetType _entitySetType;

        public override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
        public EntitySetType EntitySetType => _entitySetType;

        public void SetEntitySetType(EntitySetType entitySetType, CombatEntity entity)
        {
            _entitySetType = entitySetType;
            _entitySetConfig = _entitySetConfigContainer.EntitySetConfigs[entitySetType];
            
            _iconImage.sprite = _entitySetConfig.SetIcon;

            if (_displayCurrentSetStacks)
            {
                _stackCountTextField.text = entity.ComponentsContainer.Get<EntitySetContainer>().GetEnitySetStacks(entitySetType).ToString();
            }
        }
        
        protected override void UpdateAllParsableText()
        {
            _tagDataContainer = GetTagDataContainerFromThresholds(_entitySetConfig);
        }

        private TooltipParseTagDataContainer GetTagDataContainerFromThresholds(EntitySetConfig setConfig)
        {
            string combinedText = string.Empty;

            foreach (EntitySetThresholdConfig thresholdConfig in setConfig.ThresholdConfigs)
            {
                combinedText += thresholdConfig.ThresholdDescriptionEffectLocKey.Localize();
            }

            return TooltipDataParser.GetTooltipTagDataFromText(combinedText);
        }
    }
}