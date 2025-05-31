using Combat;

public sealed class PiercingArrows : EntityModificator
{
    private PiercingArrowsWeaponPoolModifier _poolModifier;
    
    public override void Enable()
    {
        _poolModifier = new PiercingArrowsWeaponPoolModifier();
        
        Entity.ComponentsContainer.Get<ArcherTower>().WeaponPool.AddWeaponModifier(_poolModifier);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<ArcherTower>().WeaponPool.AddWeaponModifier(_poolModifier);
    }
    
    protected sealed class PiercingArrowsWeaponPoolModifier : WeaponPoolModifier
    {
        public override void AddWeaponModification(WeaponBase weapon)
        {
            ((Arrow)weapon).SetPiercingState(true);
        }

        public override void RemoveWeaponModification(WeaponBase weapon)
        {
            ((Arrow)weapon).SetPiercingState(false);
        }
    }
}