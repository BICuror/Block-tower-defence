using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityModificationEffectData _modificationEffectData;
    
    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;
    public void SetEffectData(EntityModificationEffectData modificationEffectData) => _modificationEffectData = modificationEffectData;
    public override void ApplyEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityModificationEffectContainer>().AddEffect(_modificationEffectData);
    }
}