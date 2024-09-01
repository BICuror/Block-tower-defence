using UnityEngine;

[CreateAssetMenu(fileName = "PermamentHealOverTime", menuName = "Effect/PermamentHealOverTime")]

public sealed class PermamentHealOverTime : PermanentOverTimeEffect
{
    [Range(0f, 1f)] [SerializeField] private float _healStrength;

    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasHealth();

    public override void ApplyTickEffectToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Health.HealByPercent(_healStrength);
    }
}
