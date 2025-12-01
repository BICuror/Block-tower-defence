using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingSelectionOptionObject : SelectionOptionObject
{
    [Inject] private DiContainer _diContainer;
    [Inject] private DraggableCreator _draggableCreator;
    [SerializeField] private Transform _buildingParent;
    [SerializeField] private DraggableObject _draggableObject;
    
    private BuildingEntity _draggablePrefab;
    private BuildingEntity _instantiatedBuilding;
    
    public override string OptionName { get; }
    public override string OptionDescription { get; }
    public override Sprite Icon { get; }


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

        _draggableObject.DraggablePickedUp += _ => ((IDraggable)_instantiatedBuilding.Draggable).PickUp();
        _draggableObject.DraggablePlaced += _ => ((IDraggable)_instantiatedBuilding.Draggable).Place();
    }
}