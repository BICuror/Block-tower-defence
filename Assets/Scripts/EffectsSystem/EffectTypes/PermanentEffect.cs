namespace Combat
{
    public abstract class PermanentEffect : Effect 
    {
        public override EffectType GetEffectType() => EffectType.Permanent;
    }
}