using Navigation;
using Combat;
using System;
using Combat.Enemies;

public sealed class MoveDiagonalyEntityModifier : EntityModificator
{
    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<NavigationAgent>();
    
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<NavigationAgent>().SetNavigationAgentNodePicker(typeof(NavigationNodePickerDiagonalPicker));
    }

    public override void Disable()
    {
        Type defaultNavigationNodePickerType = Type.GetType(Entity.ComponentsContainer.Get<EnemyBootstrap>().EnemyConfig.NavigationData.NavgationNodePickerType);
        
        Entity.ComponentsContainer.Get<NavigationAgent>().SetNavigationAgentNodePicker(defaultNavigationNodePickerType);
    }
}