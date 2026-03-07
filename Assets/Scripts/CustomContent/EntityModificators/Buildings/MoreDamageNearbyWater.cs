using UnityEngine;
using Zenject;

public sealed class MoreDamageNearbyWater : EntityModificator
{
    [Inject] private IslandDataContainer _islandDataContainer;
    private EntityCanvasIcon _entityCanvasIcon;
    private LayerSetting _anyTerrainLayerSetting;
    private StatModifier _statModifier;
    private GameObject _areaDisplay;
    
    public override void Enable()
    {
        _statModifier = new();
        Entity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);
        
        Entity.Draggable.Placed += CalculateBonusDamage;

        _anyTerrainLayerSetting = Args.GetArgument<LayerSetting>("AnyTerrainLayer");
        
        _areaDisplay = Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().InstantiateAndAddModificator(Args.GetArgument<GameObject>("AreaPrefab"));
        
        CalculateBonusDamage();
    }

    private void CalculateBonusDamage()
    {
        int emptyTilesNearby = TileMap.CountValidPositionsInRadius(IsEmptyTile, new Vector2Int(Mathf.RoundToInt(Entity.transform.position.x), Mathf.RoundToInt(Entity.transform.position.z)), Entity.StatContainer.Get<ReachAreaScale>().RoundedValue);
        
        _statModifier.SetMultiplier(emptyTilesNearby * Args.GetArgument<float>("MultiplierPerTile"));

        UpdateUIIcon(emptyTilesNearby);
    }

    private void UpdateUIIcon(int emptyTilesNearby)
    {
        if (emptyTilesNearby > 0 && !_entityCanvasIcon) _entityCanvasIcon = AddIcon(true, emptyTilesNearby);
        else if (emptyTilesNearby <= 0 && _entityCanvasIcon) RemoveIcon(_entityCanvasIcon);
        
        if (_entityCanvasIcon) _entityCanvasIcon.SetValue(emptyTilesNearby);
    }

    private bool IsEmptyTile(Vector2Int position) => !TileMap.HasTile(position, _anyTerrainLayerSetting);

    public override void Disable()
    {
        if (_entityCanvasIcon) RemoveIcon(_entityCanvasIcon);
        
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().RemoveAndDestroyModificator(_areaDisplay);
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
        Entity.Draggable.Placed -= CalculateBonusDamage;
    }
}