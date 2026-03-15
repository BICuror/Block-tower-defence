using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

public sealed class CreateTemporaryBuilding : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private DraggableCreator _draggableCreator;
    private DraggableObject _createdTower;
    
    public override void Enable()
    {
        _waveStateMachine.StateEnded += OnStateQuitStarted;
    }

    private void OnStateQuitStarted(WaveState waveState) => CreateOrDestroyBoosterTower(waveState).Forget();
    private async UniTask CreateOrDestroyBoosterTower(WaveState waveState)
    {
        if (waveState == WaveState.Idle)
        {
            _createdTower = await _draggableCreator.CreateDraggableOnRandomPosition(Args.GetArgument<GameObject>("TowerPrefab").GetComponent<DraggableObject>(), Entity.transform.position);
        }
        else if (waveState == WaveState.Attack)
        {
            TryDestroyTower().Forget();
        }
    }

    public override void Disable()
    {
        _waveStateMachine.StateEnded -= OnStateQuitStarted;
        TryDestroyTower().Forget();
    }

    private async UniTask TryDestroyTower()
    {
        if (_createdTower)
        {
            await UniTask.WaitUntil(() => _createdTower.GetComponent<DraggableObject>().IsPlaced);
            
            _createdTower.GetComponent<BuildingEntity>().Health.Die();
        }
    }
}