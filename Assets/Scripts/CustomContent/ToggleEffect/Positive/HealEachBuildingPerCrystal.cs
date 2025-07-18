using Zenject;
using Combat;

public sealed class HealEachBuildingPerCrystal : GlobalToggleEffect
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private ItemsContainer _itemsContainer;
    
    public override void Enable()
    {
        _waveStateMachine.StateEnded += TryToHeal;
    }

    private void TryToHeal(WaveState waveState)
    {
        if (waveState == WaveState.Idle)
        {
            foreach (BuildingEntity buildingEntity in _globalBuildingContainer.Entities)
            {
                buildingEntity.Health.ReceivePercentHeal(Args.GetArgument<float>("HealPercent") * _itemsContainer.ContainedItems.Count);
            }
        }
    }

    public override void Disable()
    {
        _waveStateMachine.StateEnded -= TryToHeal;
    }
}