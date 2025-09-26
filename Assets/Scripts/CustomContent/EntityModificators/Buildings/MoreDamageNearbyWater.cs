using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class MoreDamageNearbyWater : EntityModificator
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private RoadMapHolder _roadMapHolder;
    private StatModifier _statModifier;
    private GameObject _areaDisplay;
    
    public override void Enable()
    {
        _statModifier = new();
        Entity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);
        Entity.Draggable.Placed += CalculateBonusDamage;
        _areaDisplay = Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().InstantiateAndAddModificator(Args.GetArgument<GameObject>("AreaPrefab"));
        _areaDisplay.transform.localScale = new Vector3(2.9f, 100f, 2.9f);
    }

    private void CalculateBonusDamage()
    {
        int emptyTilesNearby = 0;

        int roundedX = (int)Entity.transform.position.x;
        int roundedZ = (int)Entity.transform.position.z;
        
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (IsEmptyTile(roundedX + x, roundedZ + z))
                {
                    emptyTilesNearby++;
                }
            }
        }
        
        _statModifier.SetMultiplier(emptyTilesNearby * Args.GetArgument<float>("MultiplierPerTile"));
    }

    private bool IsEmptyTile(int x, int z)
    {
        if (x < 0 || x >= _islandDataContainer.Data.IslandSize || z < 0 || z >= _islandDataContainer.Data.IslandSize) return true;

        if (_roadMapHolder.Map[x, z] || _islandHeightMapHolder.Map[x, z] > 0) return false;

        return true;
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().RemoveAndDestroyModificator(_areaDisplay);
        Entity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
        Entity.Draggable.Placed -= CalculateBonusDamage;
    }
}