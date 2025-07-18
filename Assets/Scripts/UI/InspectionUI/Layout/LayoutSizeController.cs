using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public sealed class LayoutSizeController : MonoBehaviour
{ 
    [SerializeField] private List<ContentSizeFitter> _contentSizeFitters;
    [SerializeField] private List<RectTransform> _controllerRectTransforms;

    public async void RecalculateLayout()
    {
        await UniTask.WaitForFixedUpdate();
        
        _contentSizeFitters.ForEach(contentSizeFitter =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.transform as RectTransform);
        });
        
        _controllerRectTransforms.ForEach(controller =>
        {
            controller.ForceUpdateRectTransforms();
        });
    }
}