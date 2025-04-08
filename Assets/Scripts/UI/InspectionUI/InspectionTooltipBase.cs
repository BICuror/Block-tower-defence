using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;
using TMPro;

public sealed class InspectionTooltipBase : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _typeTextField;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    
    [Header("links")]
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private StatTooltip _statTooltipPrefab;
    [SerializeField] private Transform _statTooltipContainer;

    [SerializeField] private CombatEntity _inspectedEntity;
    
    private Inspectable _inspectable;

    private void Start() => SetInspectable(_inspectedEntity.ComponentsContainer.Get<Inspectable>());
    
    public void SetInspectable(Inspectable inspectable)
    {
        _inspectable = inspectable;
        _nameTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Name);
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Description);
        _typeTextField.text = _inspectable.InspectableType.ToString();
        _inspectionSubpanelsController._inspectedEntity = _inspectedEntity;
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
        
        inspectable.GetComponent<CombatEntity>().StatContainer.GetAllStats().ForEach(stat =>
        {
            StatTooltip tooltip = Instantiate(_statTooltipPrefab, _statTooltipContainer);
            tooltip.SetStat(stat);
            tooltip.TooltipClosed += () => _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        });
    }
}
