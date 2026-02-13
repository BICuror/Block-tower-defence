using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingSelectionOptionObject : SelectionOptionObject
{
    [Inject] private DiContainer _diContainer;
    [Inject] private DraggableCreator _draggableCreator;
    [SerializeField] private Transform _buildingParent;
    [SerializeField] private Transform _visualEffect;
    [SerializeField] private DraggableObject _draggableObject;
    
    [Header("BuildingOutline")]
    [SerializeField] private Color _buildingOutlineColor;
    [SerializeField] private float _buildingOutlineThickness;
    
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
        Destroy(_instantiatedBuilding.ComponentsContainer.Get<Rigidbody>());
        
        _instantiatedBuilding.ComponentsContainer.Get<BuildingDraggable>().SetDraggableState(false);
        GetComponent<DraggableObject>().SetNewDragAnimationObject(_instantiatedBuilding.ComponentsContainer.Get<DragAnimationObject>());
        
        _visualEffect.SetParent(_instantiatedBuilding.ComponentsContainer.Get<DragAnimationObject>().transform);
        _visualEffect.localPosition = Vector3.zero;
        
        _draggableObject.DraggablePickedUp += _ => ((IDraggable)_instantiatedBuilding.Draggable).PickUp();
        _draggableObject.DraggablePlaced += _ => ((IDraggable)_instantiatedBuilding.Draggable).Place();

        Outline buildingOutline = _instantiatedBuilding.ComponentsContainer.Get<DragAnimationObject>().GetComponent<Outline>();
        buildingOutline.OutlineColor = _buildingOutlineColor;
        buildingOutline.OutlineWidth = _buildingOutlineThickness;
    }
}