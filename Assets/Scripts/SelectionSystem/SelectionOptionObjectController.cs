using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using Random = UnityEngine.Random;

public sealed class SelectionOptionObjectController : MonoBehaviour
{
    [SerializeField] private float _creationRadius = 5f;
    [Inject] private DraggableCreator _draggableCreator;
    
    private readonly List<SelectionOptionObject> _selectionOptionObjects = new();
    
    public void DestroyAllCreatedSelectionOptions()
    {
        _selectionOptionObjects.ForEach(optionObject => Destroy(optionObject.gameObject));
        _selectionOptionObjects.Clear();
    }

    public async UniTask CreateSelectionOptionObjects<T>(T selectionOptionObjectPrefab, int amount, Action<T> initializeSelectionOption) where T : SelectionOptionObject
    {
        float angleStep = 360f / amount;
        float offset = Random.Range(0, 360f);

        for (int i = 0; i < amount; i++)
        {
            float xOffset = Mathf.Cos(Mathf.Deg2Rad * (angleStep * i + offset)) * _creationRadius;
            float zOffset = Mathf.Sin(Mathf.Deg2Rad * (angleStep * i + offset)) * _creationRadius;
            
            DraggableObject draggableObjectPrefab = selectionOptionObjectPrefab.GetComponent<DraggableObject>();

            T selectionOptionObject = (await _draggableCreator.CreateDraggableOnNearbyPosition(draggableObjectPrefab, transform.position, new Vector3(xOffset, 0f, zOffset) + transform.position)).GetComponent<T>();
            
            initializeSelectionOption.Invoke(selectionOptionObject);
            
            _selectionOptionObjects.Add(selectionOptionObject);
        }
    }
}