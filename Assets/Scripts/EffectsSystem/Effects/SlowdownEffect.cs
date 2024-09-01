using UnityEngine;

[CreateAssetMenu(fileName = "SlowdownEffect", menuName = "Effect/SlowdownEffect")]

public sealed class SlowdownEffect : PermanentEffect
{
    [Range(0f, 1f)] [SerializeField] private float _slowdownStrength;
    public float SlowdownStrength => _slowdownStrength;

    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasNavMeshAgent();

    public override void ApplyToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Agent.speed -= componentsContainer.Agent.speed * _slowdownStrength;
    }

    public override void RemoveFromEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Agent.speed += componentsContainer.Agent.speed / (1f - _slowdownStrength) * _slowdownStrength;
    }
}