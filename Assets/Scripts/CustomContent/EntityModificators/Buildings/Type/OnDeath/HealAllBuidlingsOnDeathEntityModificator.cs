using Zenject;
using Combat;

public sealed class HealAllBuidlingsOnDeathEntityModificator : EntityModificator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    private float _healPercent;
    
    public override void Enable()
    {
        _healPercent = Args.GetArgument<float>("HealPercent");

        Entity.Health.Died += HealAllBuildings;
    }

    private void HealAllBuildings()
    {
        foreach (BuildingEntity buildingEntity in _globalBuildingContainer.Entities)
        {
            if (buildingEntity != Entity) buildingEntity.Health.ReceivePercentHeal(_healPercent);
        }
    }
    
    public override void Disable()
    {
        Entity.Health.Died -= HealAllBuildings;
    }
}