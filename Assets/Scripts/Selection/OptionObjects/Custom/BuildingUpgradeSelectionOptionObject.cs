using UnityEngine;
using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityModificatorData _modificatorData;
    private EffectInspectionTooltipPreview _effectPreviewTooltip;
    
    public override string OptionName => _modificatorData.ModificatorName;
    public override string OptionDescription => _modificatorData.ModificatorDescription;
    public override Sprite Icon => _modificatorData.Icon;

    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;

    public async void SetEffectData(EntityModificatorData modificatorData)
    {
        _modificatorData = modificatorData;

        _effectPreviewTooltip = await InspectionTooltipManager.Instance.OpenEffectPreviewTooltip(this);
    }
    
    public override void ApplyEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_modificatorData);
    }

    private void OnDestroy()
    {
        InspectionTooltipManager.Instance.DestroyElement(_effectPreviewTooltip);
    }
}