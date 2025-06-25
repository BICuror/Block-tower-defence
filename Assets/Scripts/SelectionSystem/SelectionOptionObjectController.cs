using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public sealed class SelectionOptionObjectController : MonoBehaviour
{
    [Inject] private DraggableCreator _draggableCreator;
    
    private readonly List<SelectionOptionObject> _selectionOptionObjects = new();
    
    public void DestroyAllCreatedSelectionOptions()
    {
        _selectionOptionObjects.ForEach(optionObject => Destroy(optionObject.gameObject));
        _selectionOptionObjects.Clear();
    }

    public async UniTask<T> CreateSelectionOptionObject<T>(T selectionObjectPrefab) where T : SelectionOptionObject
    {
        DraggableObject draggableObjectPrefab = selectionObjectPrefab.GetComponent<DraggableObject>();

        T selectionOptionObject = (await _draggableCreator.CreateDraggableOnRandomPosition(draggableObjectPrefab, transform.position, 5)).GetComponent<T>();

        _selectionOptionObjects.Add(selectionOptionObject);

        return selectionOptionObject;
    }
}