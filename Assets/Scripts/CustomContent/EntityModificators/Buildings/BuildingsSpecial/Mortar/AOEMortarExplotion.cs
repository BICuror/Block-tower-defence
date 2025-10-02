public sealed class AOEMortarExplotion : EntityModificator
{
    private AOEBehaviour _behaviour;
    
    public override void Enable()
    {
        _behaviour = new AOEBehaviour(Args);
        
        if (Args.GetArgument<bool>("ReplaceMainBehavior"))
        {
            Entity.ComponentsContainer.Get<MortarTower>().CoreLanded.AddBehaviour(_behaviour, BehaviourType.Override);       
        }
        else
        {
            Entity.ComponentsContainer.Get<MortarTower>().CoreLanded.AddBehaviour(_behaviour, BehaviourType.Additional);     
        }
    }

    public override void Disable()
    {
        if (Args.GetArgument<bool>("ReplaceMainBehavior"))
        {
            Entity.ComponentsContainer.Get<MortarTower>().CoreLanded.RemoveOverrideBehaviour();       
        }
        else
        {
            Entity.ComponentsContainer.Get<MortarTower>().CoreLanded.RemoveAdditionalBehaviour(_behaviour);     
        }
    }
}