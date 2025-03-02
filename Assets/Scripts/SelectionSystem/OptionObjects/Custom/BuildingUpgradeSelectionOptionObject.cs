using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityEffectData _effectData;
    
    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;
    public void SetEffectData(EntityEffectData effectData) => _effectData = effectData;
    public override void ApplyEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityEffectContainer>().AddEffect(_effectData);
    }
}