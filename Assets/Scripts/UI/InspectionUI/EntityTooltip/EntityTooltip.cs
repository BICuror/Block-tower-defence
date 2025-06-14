using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Combat;
using TMPro;

public sealed class EntityTooltip : InspectionPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;

    [Header("Links")] 
    [SerializeField] private TooltipStatContainer _tooltipStatContainer;
    [SerializeField] private LayoutSizeController _layoutSizeController;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private EntityModificatorTooltip _entityModificatorTooltipPrefab;
    [SerializeField] private Transform _entityModificatorTooltipParent;
    
    private Inspectable _inspectable;

    public void SetInspectable(Inspectable inspectable)
    {
        CombatEntity entity = inspectable.GetComponent<CombatEntity>();
        _inspectable = inspectable;
        
        _nameTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Name);
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Description);
        _inspectionSubpanelsController.SetInspectedEntity(entity);

        bool isEnemyEntity = entity is EnemyEntity;
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));

        InitializeTooltipStatContainer(entity);
        
        inspectable.GetComponent<EntityModificatorsContainer>().AppliedModificators.OrderBy(data => data.EffectType == EffectType.Positive).ToList().ForEach(modificatorData =>
        {
            EntityModificatorTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
            
            tooltip.SetEntityModificator(modificatorData);
            tooltip.TooltipClosed += () => _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
            tooltip.SetAmount(inspectable.GetComponent<EntityModificatorsContainer>().GetModificatorsAmount(modificatorData));
        });

        _layoutSizeController.RecalculateLayout();
    }

    private void InitializeTooltipStatContainer(CombatEntity entity)
    {
        _tooltipStatContainer.SetInspectedEntity(entity);
        _tooltipStatContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipStatContainer.TooltipClosed += ReturnToDefaultInspectionState;
    }

    private void ReturnToDefaultInspectionState()
    {
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
    }
}