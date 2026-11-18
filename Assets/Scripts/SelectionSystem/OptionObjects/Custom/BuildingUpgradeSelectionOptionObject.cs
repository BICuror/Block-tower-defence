using Combat.IngameUI.Inspection.InspectionCanvas;
using Combat.IngameUI.Inspection;
using Combat.Entity.Modificators;
using Cysharp.Threading.Tasks;
using Combat;

public sealed class BuildingUpgradeSelectionOptionObject : SelectionOptionObject
{
    private BuildingEntity _targetBuildingEntity;
    private EntityModificatorConfig _modificatorConfig;
    private EntityModificatorInspectionCanvasPreview _entityModificatorPreviewCanvas;
    
    public EntityModificatorConfig ModificatorConfig => _modificatorConfig;
    public BuildingEntity TargetBuildingEntity => _targetBuildingEntity;
    
    public void SetTargetBuildingEntity(BuildingEntity buildingEntity) => _targetBuildingEntity = buildingEntity;

    public void SetEffectData(EntityModificatorConfig modificatorConfig)
    {
        _modificatorConfig = modificatorConfig;
    }
    
    public override void ApplySelectedEffect()
    {
        _targetBuildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_modificatorConfig);
    }
    
    public override void OnObjectCreationCompleted()
    {
        _entityModificatorPreviewCanvas = InspectionCanvasManager.Instance.OpenEffectPreviewTooltip(_modificatorConfig, transform);
    }

    private void OnDestroy()
    {
        InspectionCanvasManager.Instance.DestroyElement(_entityModificatorPreviewCanvas).Forget();
    }
}