using UnityEngine;

public sealed class InspectionTooltipManager : MonoBehaviour
{
    [SerializeField] private EntityTooltip _entityTooltip;

    public bool NonIdleTooltipsOpened => _entityTooltip.gameObject.activeSelf;
    
    public void ActivateEntityTooltip(Inspectable inspectable)
    {
        CloseAllTooltips();
        _entityTooltip.gameObject.SetActive(true);
        _entityTooltip.SetInspectable(inspectable);
    }
    
    public void CloseAllTooltips()
    {
        _entityTooltip.gameObject.SetActive(false);
    }
}