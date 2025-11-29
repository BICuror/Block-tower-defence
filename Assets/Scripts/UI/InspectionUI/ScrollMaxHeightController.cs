using UnityEngine;

public sealed class ScrollMaxHeightController : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private float _maxHeight;

    public void UpdateHeight()
    {
        float contentPaneltHeight = _contentParent.sizeDelta.y;

        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, Mathf.Min(_maxHeight, contentPaneltHeight));
    }
}