using System;

public sealed class EntityModificatorStatConvert : EntityModificator
{
    private StatModifier _statModifier;
    private Type _createdStatType;
    private Type _mainStatType;
    private bool _createdStat;
    
    public override void Enable()
    {
        _createdStatType = Type.GetType(Args.GetArgument<string>("CreatedStatType"));
        _mainStatType = Type.GetType(Args.GetArgument<string>("MainStatType"));
        
        if (!Entity.StatContainer.Has(_createdStatType))
        {
            _createdStat = true;
            
            Entity.StatContainer.AddStat(Activator.CreateInstance(_createdStatType) as Stat);
        }
        else
        {
            Entity.StatContainer.Get(_createdStatType).AddStatModifier(_statModifier);
        }

        Entity.StatContainer.Get(_mainStatType).ValueChanged += UpdateStatValue;
        UpdateStatValue(Entity.StatContainer.Get(_mainStatType).Value);
    }

    private void UpdateStatValue(float value)
    {
        if (_createdStat)
        {
            Entity.StatContainer.Get(_createdStatType).SetDefault(value);
        }
        else
        {
            _statModifier.SetFlat(value);
        }
    }

    public override void Disable()
    {
        Entity.StatContainer.Get(_mainStatType).ValueChanged -= UpdateStatValue;
        
        if (_createdStat)
        {
            
            Entity.StatContainer.Remove(_createdStatType);
        }
        else
        {
            Entity.StatContainer.Get(_createdStatType).RemoveStatModifier(_statModifier);
        }
    }
}
