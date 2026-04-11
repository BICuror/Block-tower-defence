using System.Collections.Generic;
using UnityEngine;

public sealed class HoverableController : MonoBehaviour
{
    [SerializeField] private LayerSetting _hoverableLayerSetting;
    [SerializeField] private Camera _camera;
    
    private List<HoverableObject> _lastHoveredOverObjects = new();
    
    public void CheckHover(Vector2 pointerPosition)
    {
        Ray cameraRay = _camera.ScreenPointToRay(pointerPosition);
        
        RaycastHit[] hits = Physics.RaycastAll(cameraRay, TileMap.RAY_LENGTH, _hoverableLayerSetting.GetLayerMask());

        List<HoverableObject> hoveredOverObjects = new();
        
        foreach (RaycastHit raycastHit in hits)
        {
            if (raycastHit.collider.gameObject.TryGetComponent(out HoverableObject hoverableObject))
            {
                hoveredOverObjects.Add(hoverableObject);
            }
        }

        for (int i = 0; i < _lastHoveredOverObjects.Count;)
        {
            if (hoveredOverObjects.Contains(_lastHoveredOverObjects[i])) i++;
            else
            {
                _lastHoveredOverObjects[i].ExitHover();
                _lastHoveredOverObjects.RemoveAt(i);
            }
        }
        
        foreach (HoverableObject hoverableObject in hoveredOverObjects)
        {
            if (!_lastHoveredOverObjects.Contains(hoverableObject))
            {
                _lastHoveredOverObjects.Add(hoverableObject);
                hoverableObject.EnterHover();
            }
        }
    }
}