using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "PoisonEffect", menuName = "Effect/Poison")]
    
    public sealed class PoisonEffect : PermanentOverTimeEffect
    {
        [SerializeField] private float _damage;
    
        public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasHealth();
    
        public override void ApplyTickEffectToEntity(EntityComponentsContainer componentsContainer)
        {
            componentsContainer.Health.ReceiveEffectDamage(_damage);
        }
    }
}