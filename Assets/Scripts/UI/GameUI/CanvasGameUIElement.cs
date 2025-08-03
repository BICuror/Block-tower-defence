using System.Collections.Generic;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(CanvasGroup))]

public abstract class CanvasGameUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<RectTransform> _contentSizeFitters;

    public Action PointerEntered;
    public Action PointerExited;
    
    protected async UniTask RebuildLayout()
    {
        _contentSizeFitters.ForEach(LayoutRebuilder.ForceRebuildLayoutImmediate);
        //_contentSizeFitters.ForEach(LayoutRebuilder.MarkLayoutForRebuild);
        await UniTask.WaitForFixedUpdate();
        await UniTask.WaitForFixedUpdate();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData _) => PointerEntered?.Invoke();

    void IPointerExitHandler.OnPointerExit(PointerEventData _) => PointerExited?.Invoke();
}