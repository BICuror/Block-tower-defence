using System.Collections.Generic;
using System;

public class EntityModificatorStatCreate : EntityModificator
{
    private List<Stat> _createdStats = new();
    
    public override bool CanBeApplied()
    {
        foreach (StatInitializer statInitializer in ModificatorData.StatInitializers)
        {
            Type statType = statInitializer.StatData.GetStatType();
            
            if (!Entity.StatContainer.Has(statType))
            {
                return true;
            }
        }

        return false;
    }
    
    public override void Enable()
    {
        ModificatorData.StatInitializers.ForEach(statInitializer =>
        {
            Type statType = statInitializer.StatData.GetStatType();
            
            if (!Entity.StatContainer.Has(statType))
            {
                Stat stat = Activator.CreateInstance(statType) as Stat;
                stat.SetDefault(statInitializer.DefaultValue);
                
                Entity.StatContainer.AddStat(stat);
            }
        });
    }

    public override void Disable()
    {
        _createdStats.ForEach(createdStat => Entity.StatContainer.Remove(createdStat.GetType()));

        _createdStats.Clear();
    }
}