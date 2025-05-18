using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Combat;

[RequireComponent(typeof(Camera))]

public class InspectorController : MonoBehaviour
{
    [SerializeField] private AreaVisualisation _areaVisualisation;
    [SerializeField] private EffectInspectionTooltip effectInspectionTooltip;
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
            if (hit.collider.gameObject.TryGetComponent(out Inspectable hoveredInspectable))
            {
                if (!_inspectable)
                {
                    StartInspecting(hoveredInspectable);
                }
    
                return true;
            }
        }

        return false;
    }
    
    public bool TryToStartInspectingItem(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (TileMap.HasTile(ray, _uiLayerSetting)) return false;

        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out Inspectable hoveredInspectable))
            {
                if (hoveredInspectable.GetComponent<Item>() == null) return false;

                if (!_inspectable)
                {
                    StartInspecting(hoveredInspectable);
                }
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
            if (_inspectable.GetComponent<Item>() != null)
            {
                if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit hit))
                {
                    Inspectable hoveredInspectable = hit.collider.GetComponent<Inspectable>();

                    if (hoveredInspectable != _inspectable && hoveredInspectable.GetComponent<Item>() != null)
                    {
                        StopInspecting();
                        StartInspecting(hoveredInspectable);
                        return false;
                    }
                }
            }

            return uiHit.collider.gameObject == _currentInspectionTooltip;
        }
        
        if (TileMap.HasTile(ray, _inspectableLayerSetting, out RaycastHit inspectableHit))
        {
            if (_inspectable.gameObject == inspectableHit.collider.gameObject) return false;
        }

        StopInspecting();
        return true;
    }

    private void StartInspecting(Inspectable inspectable)
    {
        _inspectable = inspectable;
        _inspectable.SetInspectedState(true);
        
        MeshRenderer[] meshRenderers = inspectable.GetComponentsInChildren<MeshRenderer>();
        float maxHeight = 0;
        
        _areaVisualisation.ActivateVisualisation(_inspectable.gameObject);
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            maxHeight = Mathf.Max(meshRenderers[i].bounds.size.y, maxHeight);
        }

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
        
        _areaVisualisation.DeactivateVisualisation(_inspectable.gameObject);
        _inspectable.SetInspectedState(false);
        _inspectable = null;
        Destroy(_currentInspectionTooltip);
    }
}