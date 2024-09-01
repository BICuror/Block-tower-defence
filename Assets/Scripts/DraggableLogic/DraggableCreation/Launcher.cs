using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public sealed class Launcher : MonoBehaviour
{
    public UnityEvent<DraggableObject> Landed;

    private DraggableObject _draggablePrefab;

    [Inject] private DiContainer _diContainer;

    public void SetDraggablePrefab(DraggableObject draggable) => _draggablePrefab = draggable;

    public void Land(Vector3 landPosition)
    {
        DraggableObject draggable = _diContainer.InstantiatePrefab(_draggablePrefab.gameObject, landPosition, Quaternion.identity, null).GetComponent<DraggableObject>();

        draggable.Place();

        Landed.Invoke(draggable);
    }
}
