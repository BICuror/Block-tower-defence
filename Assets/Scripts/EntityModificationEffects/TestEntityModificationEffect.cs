public sealed class TestEntityModificationEffect : EntityModificationEffect
{
    public override void Enable()
    {
        Entity.StatContainer.Get<ReachAreaScale>().ChangeFlat(10);
    }

    public override void Disable()
    {
        Entity.StatContainer.Get<ReachAreaScale>().ChangeFlat(-10);
    }
}