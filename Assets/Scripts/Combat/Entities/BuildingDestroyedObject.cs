using UnityEngine;
using Zenject;
using Combat;
using Cysharp.Threading.Tasks;

public sealed class BuildingDestroyedObject : MonoBehaviour
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [SerializeField] private InspectableObject _inspectableObject;
    [SerializeField] private DraggableObject _draggableObject;
    private BuildingEntity _buildingEntity;
    private bool _destroyedOnDeath;
    
    public bool CanBeRevived => _draggableObject.IsPlaced;

    public void SetEntity(BuildingEntity entity, bool destroyOnDeath)
    {
        _buildingEntity = entity;
        _destroyedOnDeath = destroyOnDeath;
        
        _inspectableObject.ReplaceableDataParser.AddOrUpdateParsableData("*", entity.ComponentsContainer.Get<InspectableObject>().Name);
        
        _draggableObject.SetDraggableState(entity.Draggable.IsDraggable());

        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted += OnWaveEnd;
    }
    
    public async UniTask Destroy()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted -= OnWaveEnd;

        await UniTask.WaitWhile(() => !_draggableObject.IsPlaced);
        
        Destroy(gameObject);
    }
    
    private void OnWaveEnd() 
    { 
        if (!_destroyedOnDeath) 
        { 
            _buildingEntity.ReviveBuilding().Forget();
        }
        else Destroy().Forget();
    }
}