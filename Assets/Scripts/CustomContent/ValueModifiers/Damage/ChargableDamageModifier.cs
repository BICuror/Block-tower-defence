using Combat.Stats;
using UnityEngine;

namespace Combat.ValueModifiers
{
    public sealed class ChargeableDamageModifier : ValueModifier
    {
        private MaxDamageMultiplier _maxDamageMultiplier;
        private float _charge;

        public override ValueModifierOrder Order => ValueModifierOrder.PreDamageCalculation;

        protected override void OnInitialize()
        {
            _maxDamageMultiplier = OwnerEntity.StatContainer.Get<MaxDamageMultiplier>();
            SetCharge(0f);
        }

        public void SetCharge(float charge) => _charge = charge;

        public override float GetMultChange(CombatEntity otherEntity, float value) => Mathf.Lerp(1, _maxDamageMultiplier.Value, _charge);
    }
}