using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using Random = UnityEngine.Random;

public sealed class SelectionOptionObjectController : MonoBehaviour
{
    [SerializeField] private RerollSelectionOptionObject _rerollSelectionOptionObject;
    [SerializeField] private float _creationRadius = 5f;
    [Inject] private DraggableCreator _draggableCreator;
    [Inject] private SelectionManager _selectionManager;
    
    private readonly List<SelectionOptionObject> _selectionOptionObjects = new();
    
    public void DestroyAllCreatedSelectionOptions()
    {
        _selectionOptionObjects.ForEach(optionObject => Destroy(optionObject.gameObject));
        _selectionOptionObjects.Clear();
    }

    public async UniTask CreateSelectionOptionObjects<T>(T selectionOptionObjectPrefab, int amount, Action<T> initializeSelectionOption) where T : SelectionOptionObject
    {
        bool createReroll = _selectionManager.RerollsLeft > 0;
        
        float angleStep = 360f / amount;
        
        if (createReroll) angleStep = 360f / (amount + 1);
        
        float offset = Random.Range(0, 360f);

        for (int i = 0; i < amount; i++)
        {
            DraggableObject draggableObjectPrefab = selectionOptionObjectPrefab.GetComponent<DraggableObject>();

            T selectionOptionObject = (await _draggableCreator.CreateDraggableOnNearbyPosition(draggableObjectPrefab, transform.position, GetOffset(i) + transform.position)).GetComponent<T>();
            
            initializeSelectionOption?.Invoke(selectionOptionObject);
            
            _selectionOptionObjects.Add(selectionOptionObject);
        }

        if (createReroll)
        {
            RerollSelectionOptionObject rerollSelectionOptionObject = (await _draggableCreator.CreateDraggableOnNearbyPosition(_rerollSelectionOptionObject.GetComponent<DraggableObject>(), transform.position, GetOffset(amount) + transform.position)).GetComponent<RerollSelectionOptionObject>();
            rerollSelectionOptionObject.Initialize();
            
            _selectionOptionObjects.Add(rerollSelectionOptionObject);
        }
        
        Vector3 GetOffset(int index)
        {
            float xOffset = Mathf.Cos(Mathf.Deg2Rad * (angleStep * index + offset)) * _creationRadius;
            float zOffset = Mathf.Sin(Mathf.Deg2Rad * (angleStep * index + offset)) * _creationRadius;

            return new Vector3(xOffset, 0f, zOffset);
        }
    }
}