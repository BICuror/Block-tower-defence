using UnityEngine;
using Zenject;
using Combat;

public sealed class BombCreatorTower : DefaultCombatTaskConditionProvider
{
    /*[Inject] private DraggableCreator _draggableCreator;

    [SerializeField] private Bomb _bombPrefab;

    private BuildingTaskCycle _buildingTaskCycle;

    private DraggableObject _draggableObject;

    private void Awake()
    {
        _buildingTaskCycle = GetComponent<BuildingTaskCycle>();
        //_buildingTaskCycle.ShouldWorkDelegate = DoesNotHasBomb;
        _buildingTaskCycle.TaskPerformed += CreateBomb;
    
        _buildingTaskCycle.TryCycle();
    }

    private bool DoesNotHasBomb() => _draggableObject == null;

    private void CreateBomb()
    {
        _buildingTaskCycle.StopRechargeProcess();

        Launcher launcher = _draggableCreator.CreateDraggableOnRandomPosition(_bombPrefab, transform.position);
    
        launcher.Landed.AddListener(SubscribeToPlacement);
    }

    private void SubscribeToPlacement(DraggableObject draggableObject)
    {
        _draggableObject = draggableObject;

        Bomb bomb = (draggableObject as Bomb);
        //bomb.SetExplotionDamage(Damage);
        bomb.Exploded.AddListener(Resume);
    }

    private void Resume()
    {
        _draggableObject = null;

        _buildingTaskCycle.TryCycle();
    }*/
}
