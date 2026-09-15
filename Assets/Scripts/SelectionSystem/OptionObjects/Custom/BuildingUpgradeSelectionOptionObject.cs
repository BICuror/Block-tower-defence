using Cysharp.Threading.Tasks;
using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityModificatorData _modificatorData;
    private EffectInspectionTooltipPreview _effectPreviewTooltip;
    
    public EntityModificatorData ModificatorData => _modificatorData;

    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;

    public void SetEffectData(EntityModificatorData modificatorData)
    {
        _modificatorData = modificatorData;
    }
    
    public override void ApplySelectedEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_modificatorData);
    }
    
    public override void OnObjectCreationCompleted()
    {
        _effectPreviewTooltip = InspectionTooltipManager.Instance.OpenEffectPreviewTooltip(_modificatorData, transform);
    }

    private void OnDestroy()
    {
        InspectionTooltipManager.Instance.DestroyElement(_effectPreviewTooltip).Forget();
    }
}