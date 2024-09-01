using UnityEngine;

[CreateAssetMenu(fileName = "InvincibilityPermament", menuName = "Effect/InvincibilityPermament")]

public sealed class InvincibilityPermament : PermanentEffect
{
    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasHealth();

    public override void ApplyToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Health.SetInvincibleState(true);
    }

    public override void RemoveFromEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Health.SetInvincibleState(false);
    }
}
