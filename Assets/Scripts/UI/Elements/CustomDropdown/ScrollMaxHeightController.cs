using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public sealed class ScrollMaxHeightController : MonoBehaviour
{
    [SerializeField] private bool _useYieldInstruction;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private float _maxHeight;

    public async UniTask UpdateHeight()
    {
        _contentParent.ForceUpdateRectTransforms();

        if (_useYieldInstruction) await UniTask.Yield();
        else await UniTask.WaitForFixedUpdate();
        
        float contentPanelHeight = _contentParent.sizeDelta.y;

        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, Mathf.Min(_maxHeight, contentPanelHeight));
        if (TryGetComponent(out LayoutElement layoutElement)) layoutElement.preferredHeight = Mathf.Min(_maxHeight, contentPanelHeight);
    }
}