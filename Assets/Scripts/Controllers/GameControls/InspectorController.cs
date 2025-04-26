using Combat;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class InspectorController : MonoBehaviour
{
    [SerializeField] private EffectInspectionTooltipl _effectInspectionTooltipl;
    [SerializeField] private CrystalInspectionTooltip _crystalInspectionTooltip;
    [SerializeField] private InspectionTooltipBase _entityInspectionTooltipPrefab;
    [SerializeField] private LayerSetting _inspectableLayerSetting;
    [SerializeField] private LayerSetting _uiLayerSetting;
    
    private GameObject _currentInspectionTooltip;
    private Inspectable _inspectable;
    
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    public bool TryToStartInspecting(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (TileMap.HasTile(ray, _uiLayerSetting)) return false;
        

        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            Inspectable hoveredInspectable = hit.collider.GetComponent<Inspectable>();
            
            if (!_inspectable)
            {
                StartInspecting(hoveredInspectable);
            }

            return true;
        }

        return false;
    }

    public bool TryStopInspecting(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (TileMap.HasTile(ray, _uiLayerSetting, out RaycastHit uiHit))
        {
            return uiHit.collider.gameObject == _currentInspectionTooltip;
        }

        StopInspecting();
        return true;
    }

    private void StartInspecting(Inspectable inspectable)
    {
        _inspectable = inspectable;

        /*MeshRenderer[] meshRenderers = inspectable.GetComponentsInChildren<MeshRenderer>();
        float maxHeight = 0;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            maxHeight = Mathf.Max(meshRenderers[i].bounds.size.y, maxHeight);
        }*/

        if (inspectable.TryGetComponent(out BuildingSelectionOptionObject buildingOptionObject))
        {
            CombatEntity building = buildingOptionObject.GetComponentInChildren(typeof(CombatEntity)) as CombatEntity;
            
            InspectionTooltipBase entityTooltip = Instantiate(_entityInspectionTooltipPrefab, inspectable.transform.position + new Vector3(0f, 1 / 2, 0f), Quaternion.identity);
            entityTooltip.SetInspectable(building.ComponentsContainer.Get<Inspectable>());

            _currentInspectionTooltip = entityTooltip.gameObject;
        }
        else if (inspectable.TryGetComponent<CombatEntity>(out CombatEntity combatEntity))
        {
            InspectionTooltipBase entityTooltip = Instantiate(_entityInspectionTooltipPrefab, inspectable.transform.position + new Vector3(0f, 1 / 2, 0f), Quaternion.identity);
            entityTooltip.SetInspectable(inspectable);

            _currentInspectionTooltip = entityTooltip.gameObject;
        }
        else if (inspectable.TryGetComponent<Item>(out Item item))
        {
            CrystalInspectionTooltip crystalInspectionTooltip = Instantiate(_crystalInspectionTooltip, inspectable.transform.position + new Vector3(0f, 1 / 2, 0f), Quaternion.identity);
            crystalInspectionTooltip.SetInspectable(inspectable);

            _currentInspectionTooltip = crystalInspectionTooltip.gameObject;
        }
    }

    public void StopInspecting()
    {
        if (!_inspectable) return;
        
        _inspectable = null;
        Destroy(_currentInspectionTooltip);
    }
}