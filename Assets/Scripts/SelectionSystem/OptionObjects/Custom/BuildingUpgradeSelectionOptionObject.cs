using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityModificatorData _modificatorData;
    private EffectInspectionTooltip _effectInspectionTooltip;
    
    public override string OptionName => _modificatorData.ModificatorName;
    public override string OptionDescription => _modificatorData.ModificatorDescription;
    
    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;

    public void SetEffectData(EntityModificatorData modificatorData)
    {
        _modificatorData = modificatorData;

        _effectInspectionTooltip = InspectionTooltipManager.Instance.CreateEffectTooltip(this);
    } 

    public override void ApplyEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(_modificatorData);
    }

    private void OnDestroy()
    {
        InspectionTooltipManager.Instance.DestroyEffectTooltip(_effectInspectionTooltip);
    }
}