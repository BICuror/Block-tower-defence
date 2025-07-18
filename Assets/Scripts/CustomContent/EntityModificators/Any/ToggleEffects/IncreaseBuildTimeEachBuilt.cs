using Zenject;

public sealed class IncreaseBuildTimeEachBuilt : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    private StatModifier _statModifier;
    
    public override void Enable()
    {
        _statModifier = new StatModifier();
        
        Entity.StatContainer.Get<BuildTime>().AddStatModifier(_statModifier);
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += IncreaseBuildTime;
    }

    private void IncreaseBuildTime()
    {
        if (_waveStateMachine.CurrentState == WaveState.Attack) _statModifier.SetMultiplier(_statModifier.Multiplier + Args.GetArgument<float>("BuildTimeIncreasePerBuilt"));
    }

    public override void Disable()
    {
        Entity.StatContainer.Get<BuildTime>().RemoveStatModifier(_statModifier);
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= IncreaseBuildTime;
    }
}