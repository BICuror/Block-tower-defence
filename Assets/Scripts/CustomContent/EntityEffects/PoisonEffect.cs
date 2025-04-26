namespace Combat
{
    public sealed class PoisonEffect : EntityTickEffect
    {
        private const float DAMAGE_PER_STACK = 1.5f;

        protected override void Tick()
        {
            Entity.Health.ReceiveEffectDamage(DAMAGE_PER_STACK * Stack);
        }
    }
}