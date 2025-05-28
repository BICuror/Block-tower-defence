using System.Linq;
using UnityEngine;
using Combat;
using TMPro;

public sealed class InspectionTooltipBase : InspectionPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    
    [Header("links")]
    [SerializeField] private LayoutSizeController _layoutSizeController;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private EntityModificatorTooltip _entityModificatorTooltipPrefab;
    [SerializeField] private Transform _entityModificatorTooltipParent;
    [SerializeField] private StatTooltip _statTooltipPrefab;
    [SerializeField] private Transform _statTooltipContainer;
    
    private Inspectable _inspectable;

    public void SetInspectable(Inspectable inspectable)
    {
        _inspectable = inspectable;
        _nameTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Name);
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Description);
        _inspectionSubpanelsController._inspectedEntity = inspectable.GetComponent<CombatEntity>();

        bool isEnemyEntity = _inspectionSubpanelsController._inspectedEntity is EnemyEntity;
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
        
        inspectable.GetComponent<CombatEntity>().StatContainer.GetAllStats().ForEach(stat =>
        {
            StatTooltip tooltip = Instantiate(_statTooltipPrefab, _statTooltipContainer);
            tooltip.SetStat(stat);
            tooltip.TooltipClosed += () => _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        });
        
        inspectable.GetComponent<EntityModificatorsContainer>().AppliedModificators.OrderBy(data => data.EffectType == EffectType.Positive).ToList().ForEach(modificatorData =>
        {
            EntityModificatorTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
            
            bool isNegativeEffect = (isEnemyEntity && modificatorData.EffectType == EffectType.Positive) || (!isEnemyEntity && modificatorData.EffectType == EffectType.Negative);
            
            tooltip.SetEntityModificator(modificatorData, isNegativeEffect);
            tooltip.TooltipClosed += () => _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
            tooltip.SetAmount(inspectable.GetComponent<EntityModificatorsContainer>().GetModificatorsAmount(modificatorData));
        });

        _layoutSizeController.RecalculateLayout();
    }
}
