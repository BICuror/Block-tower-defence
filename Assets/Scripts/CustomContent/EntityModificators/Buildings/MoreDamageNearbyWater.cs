using UnityEngine;
using Zenject;

public sealed class MoreDamageNearbyWater : EntityModificator
{
    [Inject] private IslandDataContainer _islandDataContainer;
    private LayerSetting _anyTerrainLayerSetting;
    private StatModifier _statModifier;
    private GameObject _areaDisplay;
    private int _radius;
    
    public override void Enable()
    {
        _statModifier = new();
        Entity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);
        
        Entity.Draggable.Placed += CalculateBonusDamage;

        _radius = Args.GetArgument<int>("AreaRadius");
        _anyTerrainLayerSetting = Args.GetArgument<LayerSetting>("AnyTerrainLayer");
        
        _areaDisplay = Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().InstantiateAndAddModificator(Args.GetArgument<GameObject>("AreaPrefab"));
        _areaDisplay.transform.localScale = new Vector3(1 + 2 * _radius, 100f, 1 + 2 * _radius);
    }

    private void CalculateBonusDamage()
    {
        int emptyTilesNearby = TileMap.CountValidPositionsInRadius(IsEmptyTile, new Vector2Int(Mathf.RoundToInt(Entity.transform.position.x), Mathf.RoundToInt(Entity.transform.position.z)), _radius);
        
        _statModifier.SetMultiplier(emptyTilesNearby * Args.GetArgument<float>("MultiplierPerTile"));
    }

    private bool IsEmptyTile(Vector2Int position) => !TileMap.HasTile(position, _anyTerrainLayerSetting);

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().RemoveAndDestroyModificator(_areaDisplay);
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
        Entity.Draggable.Placed -= CalculateBonusDamage;
    }
}