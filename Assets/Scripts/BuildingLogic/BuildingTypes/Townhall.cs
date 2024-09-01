using UnityEngine;
using Zenject;

public sealed class Townhall : BuildingHealth
{
    [SerializeField] private DraggableObject[] _draggablesToCreateOnStart;
    [Inject] private DraggableCreator _draggableCreator;

    private void Awake()
    {
        GetComponent<IDraggable>().Place();
        base.Awake();        
    }

    private void Start()
    {
        for (int i = 0; i < _draggablesToCreateOnStart.Length; i++)
        {
            _draggableCreator.CreateDraggableOnRandomPosition(_draggablesToCreateOnStart[i], transform.position, 4);
        }
        base.Start();
    }
    
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition + Vector3.up;
        FindObjectOfType<CameraRotationController>().SetTarget(transform);
    }

    public override void Die()
    {
        DeathEvent.Invoke(gameObject);

        BuildingDeathEvent.Invoke(this);
    }
}

