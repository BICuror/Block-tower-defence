using Configs;

namespace Combat.ValueModifiers
{
    public sealed class DefaultValueModifier : ValueModifier
    {
        [Argument("FlatChange")] private float _flatChange; 
        [Argument("MultChange")] private float _multChange;

        public override float GetFlatChange(CombatEntity otherEntity, float value) => _flatChange;

        public override float GetMultChange(CombatEntity otherEntity, float value) => _multChange;
    }
}