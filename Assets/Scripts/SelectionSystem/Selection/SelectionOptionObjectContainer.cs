using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SelectionOptionObjectContainer : MonoBehaviour
{   
    [SerializeField] private SelectionOptionObject _selectionOptionObjectPrefab;
    private ObjectPool<SelectionOptionObject> _objectPool;

    private void Awake()
    {
        _objectPool = new ObjectPool<SelectionOptionObject>(_selectionOptionObjectPrefab, 3);
    }

    public List<SelectionOptionObject> GetSelectionObjects(int amount)
    {
        List<SelectionOptionObject> result = new();

        for (int i = 0; i < amount; i++)
        {
            result.Add(_objectPool.GetNextPooledObject());
        }

        return result;
    } 
}
