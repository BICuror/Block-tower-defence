using UnityEngine;

public sealed class PointFollowerUI : MonoBehaviour
{
    [SerializeField] private Transform _uiTargetPoint;
    private Transform _target;
    
    private void Awake()
    {
        IngameUIElementManager.Instance.StaticElementsUpdated += UpdatePosition;
    }
    
    private void SetTarget(Transform target)
    {
        _target = target;
    }

    private void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(_target.position) - _target.localPosition;
    }

    private void OnDisable()
    {
        IngameUIElementManager.Instance.StaticElementsUpdated -= UpdatePosition;
    }
}