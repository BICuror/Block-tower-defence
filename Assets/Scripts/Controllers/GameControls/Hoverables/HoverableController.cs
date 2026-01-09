using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class HoverableController : MonoBehaviour
{
    [SerializeField] private LayerSetting _hoverableLayerSetting;
    private List<HoverableObject> _hoveredOverObjects = new();
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    public void CheckHover(Vector2 pointerPosition)
    {
        Debug.Log("Hover tried");
        Ray cameraRay = _camera.ScreenPointToRay(pointerPosition);
        
        RaycastHit[] hits = Physics.RaycastAll(cameraRay, TileMap.RAY_LENGTH, _hoverableLayerSetting.GetLayerMask());

        List<HoverableObject> hoveredObjects = new();
        
        foreach (RaycastHit raycastHit in hits)
        {
            if (raycastHit.collider.gameObject.TryGetComponent(out HoverableObject hoverableObject))
            {
                hoveredObjects.Add(hoverableObject);
            }
        }
        
        if (hits.Length > 0)
        {
            for (int i = 0; i < _hoveredOverObjects.Count;)
            {
                if (hoveredObjects.Contains(_hoveredOverObjects[i])) i++;
                else
                {
                    _hoveredOverObjects[i].ExitHover();
                    _hoveredOverObjects.RemoveAt(i);
                }
            }
        }
        
        foreach (HoverableObject hoverableObject in hoveredObjects)
        {
            if (!_hoveredOverObjects.Contains(hoverableObject))
            {
                _hoveredOverObjects.Add(hoverableObject);
                hoverableObject.EnterHover();
            }
        }
    }
}