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
    [SerializeField] private EffectInspectionTooltip _effectTooltipPrefab;
    private readonly List<EffectInspectionTooltip> _instantiatedEffectTooltips = new();
    
    public bool NonIdleTooltipsOpened => _entityTooltip.gameObject.activeSelf || _crystalInspectionTooltip.gameObject.activeSelf;

    private void Awake()
    {
        _instance = this;
        
        _dragController.PickedObject.AddListener(_ => SetEffectTooltipStates(false));
        _dragController.DroppedObject.AddListener(_ => SetEffectTooltipStates(true));
    }
    
    public void ActivateEntityTooltip(CombatEntity entity)
    {
        CloseAllTooltips();
        _entityTooltip.gameObject.SetActive(true);
        _crystalInspectionTooltip.gameObject.SetActive(false);
        _entityTooltip.SetEntity(entity);
        SetEffectTooltipStates(false);
    }

    public void ActivateCrystalTooltip(Item item)
    {
        CloseAllTooltips();
        _entityTooltip.gameObject.SetActive(false);
        _crystalInspectionTooltip.gameObject.SetActive(true);
        _crystalInspectionTooltip.SetItem(item);
        SetEffectTooltipStates(false);
    }

    public EffectInspectionTooltip CreateEffectTooltip(SelectionOptionObject selectionOptionObject)
    {
        EffectInspectionTooltip effectTooltip = Instantiate(_effectTooltipPrefab, _entityTooltip.transform.parent);
        effectTooltip.SetSelectionOptionObject(selectionOptionObject);
        
        _instantiatedEffectTooltips.Add(effectTooltip);
        
        return effectTooltip;
    }

    public void DestroyEffectTooltip(EffectInspectionTooltip effectTooltip)
    {
        _instantiatedEffectTooltips.Remove(effectTooltip);
        Destroy(effectTooltip.gameObject);
    }
    
    private void SetEffectTooltipStates(bool state)
    {
        _instantiatedEffectTooltips.ForEach(effectTooltip => effectTooltip.gameObject.SetActive(state));
    }
    
    public void CloseAllTooltips()
    {
        _entityTooltip.gameObject.SetActive(false);
        _crystalInspectionTooltip.gameObject.SetActive(false);
        SetEffectTooltipStates(true);
    }
}