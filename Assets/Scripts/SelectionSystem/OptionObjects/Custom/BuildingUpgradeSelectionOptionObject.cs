using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityModificatorData _modificatorData;
    private EffectInspectionTooltipPreview _effectPreviewTooltip;
    
    public EntityModificatorData ModificatorData => _modificatorData;

    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;

    public async void SetEffectData(EntityModificatorData modificatorData)
    {
        _modificatorData = modificatorData;

        _effectPreviewTooltip = await InspectionTooltipManager.Instance.OpenEffectPreviewTooltip(_modificatorData, transform);
    }
    
    public override void ApplySelectedEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_modificatorData);
    }

    private void OnDestroy()
    {
        InspectionTooltipManager.Instance.DestroyElement(_effectPreviewTooltip);
    }
}