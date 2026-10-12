using Cysharp.Threading.Tasks;
using Configs;

public sealed class ChangeStatFlatCommand : Command, INonExecutableCommand
{
    [Argument("StatData")] private StatData _statData;
    [Argument("FlatChange")] private float _flatChange;
    
    private StatModifier _statModifier = new StatModifier();
    
    public override UniTask Enable()
    {
        _statModifier.SetFlat(_flatChange);
        
        Args.Owner.StatContainer.Get(_statData.GetStatType()).AddStatModifier(_statModifier);
        
        return UniTask.CompletedTask;
    }

    public override UniTask Disable()
    {
        Args.Owner.StatContainer.Get(_statData.GetStatType()).RemoveStatModifier(_statModifier);
        
        return UniTask.CompletedTask;
    }
}

public sealed class ChangeStatMultCommand : Command, INonExecutableCommand
{
    [Argument("StatData")] private StatData _statData;
    [Argument("MultiplierChange")] private float _multiplierChange;
    
    private StatModifier _statModifier = new StatModifier();

    public override UniTask Enable()
    {
        _statModifier.SetMultiplier(_multiplierChange);
        
        Args.Owner.StatContainer.Get(_statData.GetStatType()).AddStatModifier(_statModifier);
        
        return UniTask.CompletedTask;
    }

    public override UniTask Disable()
    {
        Args.Owner.StatContainer.Get(_statData.GetStatType()).RemoveStatModifier(_statModifier);
        
        return UniTask.CompletedTask;
    }
}