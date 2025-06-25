using Cysharp.Threading.Tasks;
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
    
    public BuildingEntity InstantiatedBuilding => _instantiatedBuilding;

    public override void ApplyEffect()
    {
        _draggableCreator.CreateDraggableOnRandomPosition(_draggablePrefab.GetComponent<DraggableObject>(), transform.position);
    }

    public void SetBuilding(BuildingEntity building)
    {
        _draggablePrefab = building;
        _instantiatedBuilding = _diContainer.InstantiatePrefab(building, transform.position, transform.rotation, _buildingParent).GetComponent<BuildingEntity>();
        Destroy(_instantiatedBuilding.GetComponent<Rigidbody>());
        
        _instantiatedBuilding.GetComponent<BuildingDraggable>().SetDraggableState(false);
        GetComponent<DraggableObject>().SetNewDragAnimationObject(_instantiatedBuilding.ComponentsContainer.Get<DragAnimationObject>());
    }
}