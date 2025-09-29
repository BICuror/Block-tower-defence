using Combat;

public sealed class PiercingArrows : EntityModificator
{
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<ArcherTower>().ArrowHitBehaviour.AddBehaviour(new IgnoreArrowCollision(), BehaviourType.Override);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<ArcherTower>().ArrowHitBehaviour.RemoveOverrideBehaviour();
    }

    private sealed class IgnoreArrowCollision : CombatBehaviour<Arrow>
    {
        public override void Execute(Arrow dynamicArg) {}
    }
}