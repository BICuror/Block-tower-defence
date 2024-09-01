using UnityEngine;

[CreateAssetMenu(fileName = "WorkFaster", menuName = "Effect/WorkFaster")]

public sealed class WorkFasterEffect : PermanentEffect
{
    [Range(0f, 1f)] [SerializeField] private float _rechargeCutdownStrength;
    public float EffectStrength => _rechargeCutdownStrength;

    public override bool CanBeApplied(EntityComponentsContainer componentsContainer) => componentsContainer.HasTaskCycle();

    public override void ApplyToEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Task.RechargeTime -= componentsContainer.Task.RechargeTime * _rechargeCutdownStrength;
    }

    public override void RemoveFromEntity(EntityComponentsContainer componentsContainer)
    {
        componentsContainer.Task.RechargeTime += componentsContainer.Task.RechargeTime / (1f - _rechargeCutdownStrength) * _rechargeCutdownStrength;
    }
}
