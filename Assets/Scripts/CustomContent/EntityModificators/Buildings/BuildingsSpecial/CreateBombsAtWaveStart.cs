using Cysharp.Threading.Tasks;
using Zenject;

public sealed class CreateBombsAtWaveStart : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= Entity.ComponentsContainer.Get<BombCreatorTower>().CreateBomb;
        _waveStateMachine.StateStarted += TryCreateAllBombs;
    }

    private async void TryCreateAllBombs(WaveState waveState)
    {
        if (waveState != WaveState.Attack) return;

        int bombsToCreate = Entity.StatContainer.Get<MaxEntities>().RoundedValue - Entity.ComponentsContainer.Get<BombCreatorTower>().ActiveBombs;

        for (int i = 0; i < bombsToCreate; i++)
        {
            Entity.ComponentsContainer.Get<BombCreatorTower>().CreateBomb();

            await UniTask.WaitForSeconds(1.5f);
        }
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Entity.ComponentsContainer.Get<BombCreatorTower>().CreateBomb;
        _waveStateMachine.StateStarted -= TryCreateAllBombs;
    }
}