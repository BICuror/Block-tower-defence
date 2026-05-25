using Combat;
using Cysharp.Threading.Tasks;

public sealed class FasterCycleUntilKill : EntityModificator
{
    private EntityCanvasBar _bar;
    private StatModifier _statModifier; 
    private float _taskMultiplierReducePerAction;
    private int _maxReduceStacks;
    private int _currentStacks;
    
    public override void Enable()
    {
        _statModifier = new StatModifier();
        
        _taskMultiplierReducePerAction = Args.GetArgument<float>("TaskMultiplierReducePerAction");
        _maxReduceStacks = Args.GetArgument<int>("MaxReduceStacks");
        
        Entity.StatContainer.Get<TaskRechargeDuration>().AddStatModifier(_statModifier);
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += ReduceTaskRechargeDuration;
        Entity.ValueModifierContainer.EntityKilled += ResetStatModifier;

        _bar = AddBar(0);
    }

    private void ReduceTaskRechargeDuration()
    {
        if (_currentStacks >= _maxReduceStacks) return;
        
        _currentStacks++;
        _statModifier.SetMultiplier(_currentStacks * _taskMultiplierReducePerAction);
        _bar.SetValue(_currentStacks / (float)_maxReduceStacks).Forget();
    }

    private void ResetStatModifier(CombatEntity _)
    {
        _currentStacks = 0;
        _statModifier.SetMultiplier(0f);
        _bar.SetValue(0f).Forget();
    }

    public override void Disable()
    {
        Entity.StatContainer.Get<TaskRechargeDuration>().RemoveStatModifier(_statModifier);
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= ReduceTaskRechargeDuration;
        Entity.ValueModifierContainer.EntityKilled -= ResetStatModifier;
        
        RemoveBar(_bar);
    }
}