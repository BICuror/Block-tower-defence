using UnityEngine;
using Zenject;
using Combat;

public sealed class BuildingDestroyedObject : MonoBehaviour
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [SerializeField] private InspectableObject _inspectableObject;
    [SerializeField] private DraggableObject _draggableObject;
    private BuildingEntity _buildingEntity;
    private bool _destroyedOnDeath;
    
    public void SetEntity(BuildingEntity entity, bool destroyOnDeath)
    {
        _buildingEntity = entity;
        _destroyedOnDeath = destroyOnDeath;
        
        _inspectableObject.ReplaceableDataParser.AddOrUpdateParsableData("*", entity.ComponentsContainer.Get<InspectableObject>().Name);
        
        _draggableObject.SetDraggableState(entity.Draggable.IsDraggable());

        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted += OnWaveEnd;
    }

    private void OnWaveEnd()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted -= OnWaveEnd;
        
        if (!_destroyedOnDeath)
        {
            _buildingEntity.transform.position = transform.position;
            _buildingEntity.BuildingHealth.ReviveBuilding();
        }
        
        Destroy(gameObject);
    }
}