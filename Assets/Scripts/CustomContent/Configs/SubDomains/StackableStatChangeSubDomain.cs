using Cysharp.Threading.Tasks;
using UnityEngine;
using Configs;

public sealed class StackableStatChangeSubDomain : SubDomain
{
    [Argument("StatData")] private StatData _statData;
    [Argument("MaxStacks")] private int _maxStacks;
    [Argument("FlatChangePerStack")] private float _flatChangePerStack;
    [Argument("MultChangePerStack")] private float _multChangePerStack;

    private StatModifier _statModifier = new();

    public void SetStacks(int stacks)
    {
        int resultStacks = Mathf.Clamp(stacks, 0, _maxStacks);
        
        _statModifier.SetFlat(_flatChangePerStack * resultStacks);
        _statModifier.SetMultiplier(_multChangePerStack * resultStacks);
    }
    
    public override UniTask Enable()
    {
        Args.Owner.StatContainer.Get(_statData.GetStatType()).AddStatModifier(_statModifier);
        
        return UniTask.CompletedTask;
    }

    public override UniTask Disable()
    {
        Args.Owner.StatContainer.Get(_statData.GetStatType()).RemoveStatModifier(_statModifier);
        
        return UniTask.CompletedTask;
    }
}

public sealed class SetStackableStatChangeStacksCommand : SubDomainCommand<StackableStatChangeSubDomain>, ICommand<int>
{
    public UniTask Execute(int args)
    {
        SubDomain.SetStacks(args);
        
        return UniTask.CompletedTask;
    }
}