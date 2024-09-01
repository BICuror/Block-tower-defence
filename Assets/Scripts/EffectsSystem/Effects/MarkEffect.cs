using UnityEngine;

[CreateAssetMenu(fileName = "MarkEffect", menuName = "Effect/MarkEffect")]

public sealed class MarkEffect : RemoveOverTimeEffect
{
    [SerializeField] private float _incomingDamageMultipluerAdded;

    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasHealth();

    public override void ApplyToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Health.ChangeIncomingDamageMultipluer(_incomingDamageMultipluerAdded);
    }
    
    public override void RemoveFromEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Health.ChangeIncomingDamageMultipluer(-_incomingDamageMultipluerAdded);
    }
}
