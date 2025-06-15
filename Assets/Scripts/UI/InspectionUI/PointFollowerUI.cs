using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PointFollowerUI : MonoBehaviour
{
    [SerializeField] private Transform _uiTargetPoint;
    [SerializeField] private RectTransform _bordersRect;
    private float _halfHeight;
    private float _halfWidth;
    private Canvas _parentCanvas;
    private Transform _target;
    Vector3[] _borderPoints = new Vector3[4];
    
    private void Awake()
    {
        _parentCanvas = transform.root.gameObject.GetComponent<Canvas>();
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        _halfHeight = rectTransform.sizeDelta.y / 2; 
        _halfWidth = rectTransform.sizeDelta.x / 2;
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
        
        Vector2 position = TransformWorldPositionToUIPosition(_target.position);
        transform.localPosition = position;
        
        UpdatePosition();
    }

    private void Update() => UpdatePosition();
    
    private async void UpdatePosition()
    {
        await UniTask.WaitForEndOfFrame();
        
        _bordersRect.GetLocalCorners(_borderPoints);
        
        Vector3 position = TransformWorldPositionToUIPosition(_target.position);

        if (position.x - _halfWidth < _borderPoints[0].x)
        {
            position.x = _borderPoints[0].x + _halfWidth;
        }
        else if (position.x + _halfWidth > _borderPoints[2].x)
        {
            position.x = _borderPoints[2].x - _halfWidth;
        }
        
        if (position.y - _halfHeight < _borderPoints[0].y)
        {
            position.y = _borderPoints[0].y + _halfHeight;
        }
        else if (position.y + _halfHeight > _borderPoints[2].y)
        {
            position.y = _borderPoints[2].y - _halfHeight;
        }

        transform.localPosition = position;
    }
    
    private Vector2 TransformWorldPositionToUIPosition(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentCanvas.transform as RectTransform, screenPos, _parentCanvas.worldCamera, out Vector2 uiPosition);
        
        return uiPosition;
    }
}