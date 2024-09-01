using UnityEngine;

[CreateAssetMenu(fileName = "OnFireEffect", menuName = "Effect/OnFireEffect")]

public sealed class OnFireEffect : RemoveOverTicksEffect
{
    [SerializeField] private float _damage;

    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasHealth();

    public override void ApplyTickEffectToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Health.GetHurt(_damage);
    }
}
