using UnityEngine;

[CreateAssetMenu(fileName = "SlowdownEffectOverTime", menuName = "Effect/SlowdownEffectOverTime")]

public sealed class SlowdownEffectOverTime : PermanentEffect
{
    [Range(0f, 1f)] [SerializeField] private float _slowdownStrength;

    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasNavMeshAgent();

    public override void ApplyToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Agent.speed -= componentsContainer.Agent.speed * _slowdownStrength;
    }

    public override void RemoveFromEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Agent.speed = componentsContainer.Agent.speed / (1f - _slowdownStrength) * 10;
    }
}