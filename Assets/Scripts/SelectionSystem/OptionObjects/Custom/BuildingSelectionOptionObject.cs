using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingSelectionOptionObject : SelectionOptionObject
{
    [Inject] private DiContainer _diContainer;
    [Inject] private DraggableCreator _draggableCreator;
    [SerializeField] private Transform _buildingParent;

    private BuildingEntity _draggablePrefab;
    private BuildingEntity _instantiatedBuilding;

    public override string OptionName { get; }
    public override string OptionDescription { get; }

    public override void ApplyEffect()
    {
        _draggableCreator.CreateDraggableOnRandomPosition(_draggablePrefab.GetComponent<DraggableObject>(), transform.position, 4);
    }

    public void SetBuilding(BuildingEntity building)
    {
        _draggablePrefab = building;
        _instantiatedBuilding = _diContainer.InstantiatePrefab(building, transform.position, transform.rotation, _buildingParent).GetComponent<BuildingEntity>();
        
        Destroy(_instantiatedBuilding.GetComponent<BuildingDraggable>());
        Destroy(_instantiatedBuilding.GetComponent<Collider>());
        Destroy(_instantiatedBuilding.GetComponent<Rigidbody>());
    }
}