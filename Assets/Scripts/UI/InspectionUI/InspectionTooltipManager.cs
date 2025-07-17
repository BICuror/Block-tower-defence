using System.Collections.Generic;
using UnityEngine;
using Combat;

public sealed class InspectionTooltipManager : MonoBehaviour
{
    private static InspectionTooltipManager _instance;
    public static InspectionTooltipManager Instance => _instance;
    
    [SerializeField] private DragController _dragController;
    [SerializeField] private EntityTooltip _entityTooltip;
    [SerializeField] private CrystalInspectionTooltip _crystalInspectionTooltip;
    [SerializeField] private EffectInspectionTooltip _effectInspectionTooltip;
    [SerializeField] private EffectInspectionTooltipPreview _effectPreviewTooltipPrefab;
    private readonly List<EffectInspectionTooltipPreview> _instantiatedEffectTooltips = new();
    private InspectionState _inspectionState = InspectionState.Idle;
    
    public bool NonIdleTooltipsOpened => _inspectionState != InspectionState.Idle;

    private void Awake()
    {
        _instance = this;
        
        _dragController.PickedObject.AddListener(_ =>
        {
            _inspectionState = InspectionState.Inspecting;
            UpdateEffectTooltipStates();
        });
        _dragController.DroppedObject.AddListener(_ =>
        {
            _inspectionState = InspectionState.Idle;
            UpdateEffectTooltipStates();
        });

        _entityTooltip.Closed += CloseAllTooltips;
        _crystalInspectionTooltip.Closed += CloseAllTooltips;
        _effectInspectionTooltip.Closed += CloseAllTooltips;
    }
    
    public void ActivateEntityTooltip(CombatEntity entity)
    {
        _inspectionState = InspectionState.Inspecting;

        _entityTooltip.Initialize(entity);
        _crystalInspectionTooltip.Disable();
        _effectInspectionTooltip.Disable();
        _entityTooltip.Enable();
        UpdateEffectTooltipStates();
    }

    public void ActivateCrystalTooltip(Item item)
    {
        _inspectionState = InspectionState.Inspecting;
        
        _crystalInspectionTooltip.Initialize(item);
        _entityTooltip.Disable();
        _effectInspectionTooltip.Disable();
        _crystalInspectionTooltip.Enable();
        UpdateEffectTooltipStates();
    }
    
    public void ActivateEffectTooltip(SelectionOptionObject selectionOptionObject)
    {
        _inspectionState = InspectionState.Inspecting;
        
        _effectInspectionTooltip.SetSelectionOptionObject(selectionOptionObject);
        _entityTooltip.Disable();
        _crystalInspectionTooltip.Disable();
        _effectInspectionTooltip.Enable();
        UpdateEffectTooltipStates();
    }

    public EffectInspectionTooltipPreview CreateEffectTooltip(SelectionOptionObject selectionOptionObject)
    {
        EffectInspectionTooltipPreview effectPreviewTooltip = Instantiate(_effectPreviewTooltipPrefab, _entityTooltip.transform.parent);
        effectPreviewTooltip.gameObject.SetActive(_inspectionState == InspectionState.Idle);
        effectPreviewTooltip.GetComponent<PointFollowerUI>().SetTarget(selectionOptionObject.transform);
        
        _instantiatedEffectTooltips.Add(effectPreviewTooltip);
        
        return effectPreviewTooltip;
    }

    public void DestroyEffectTooltip(EffectInspectionTooltipPreview effectPreviewTooltip)
    {
        _instantiatedEffectTooltips.Remove(effectPreviewTooltip);
        Destroy(effectPreviewTooltip.gameObject);
    }
    
    private void UpdateEffectTooltipStates()
    {
        if (_inspectionState == InspectionState.Idle)
        {
            _instantiatedEffectTooltips.ForEach(effectTooltip => effectTooltip.Enable());
        }
        else
        {
            _instantiatedEffectTooltips.ForEach(effectTooltip => effectTooltip.Disable());
        }
    }
    
    public void CloseAllTooltips()
    {
        _inspectionState = InspectionState.Idle;
        
        _entityTooltip.Disable();
        _crystalInspectionTooltip.Disable();
        _effectInspectionTooltip.Disable();
        
        UpdateEffectTooltipStates();
    }

    private enum InspectionState
    {
        Idle,
        Inspecting
    }
}