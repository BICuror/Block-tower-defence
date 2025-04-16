using System.Collections.Generic;
using UnityEngine;

public sealed class LayoutSizeController : MonoBehaviour
{
    [Tooltip("Layout resize goes from first element to last")]
    [SerializeField] private List<RectTransform> _controllerRectTransforms;

    public void RecalculateLayout()
    {
        for (int i = 0; i < _controllerRectTransforms.Count; i++)
        {
            _controllerRectTransforms[i].ForceUpdateRectTransforms();
        }
    }
}