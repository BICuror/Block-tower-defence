namespace Combat
{
    public sealed class EnemyEntity : CombatEntity
    {
        public EnemyHealth Health => CachedComponentsContainer.Get<EnemyHealth>();
    }
}