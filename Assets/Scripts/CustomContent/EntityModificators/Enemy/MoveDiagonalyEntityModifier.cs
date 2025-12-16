using Navigation;
using Combat;
using System;

public sealed class MoveDiagonalyEntityModifier : EntityModificator
{
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<NavigationAgent>().SetNavigationAgentNodePicker(typeof(NavigationNodePickerDiagonalPicker));
    }

    public override void Disable()
    {
        Type defaultNavigationNodePickerType = Type.GetType(Entity.ComponentsContainer.Get<EnemyBootstrap>().EnemyData.NavigationData.NavgationNodePickerType);
        
        Entity.ComponentsContainer.Get<NavigationAgent>().SetNavigationAgentNodePicker(defaultNavigationNodePickerType);
    }
}