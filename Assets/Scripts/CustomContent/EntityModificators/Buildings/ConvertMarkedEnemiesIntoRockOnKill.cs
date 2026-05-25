using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

public sealed class ConvertMarkedEnemiesIntoRockOnKill : EntityModificator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private WaveStateMachine _waveStateMachine;
    private List<BuildingEntity> _createdBuildings = new();
    
    public override void Enable()
    {
        Entity.ValueModifierContainer.EntityKilled += SpawnRock;
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted += DestroyAllTowers;
    }

    public override void Disable()
    {
        DestroyAllTowers();
        
        Entity.ValueModifierContainer.EntityKilled -= SpawnRock;
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted -= DestroyAllTowers;
    }

    private void SpawnRock(CombatEntity killedEntity)
    {
        if (!killedEntity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(typeof(MarkEffect))) return;
        
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(killedEntity.transform.position.x), Mathf.RoundToInt(killedEntity.transform.position.z));
        
        if (TileMap.HasTile(roundedPosition, Args.GetArgument<LayerSetting>("SolidObjectsLayer"))) return;

        int height = _islandHeightMapHolder.Map[roundedPosition.x, roundedPosition.y];
        if (height < 1) height = 1;

        height++;
        
        Vector3 spawnPosition = new Vector3(roundedPosition.x, height, roundedPosition.y);
            
        BuildingEntity createdTower = Object.Instantiate(Args.GetArgument<GameObject>("RockTowerPrefab"), spawnPosition, Quaternion.identity).GetComponent<BuildingEntity>();
        
        _globalBuildingContainer.Add(createdTower);
        
        _createdBuildings.Add(createdTower);
    }

    private void DestroyAllTowers()
    {
        _createdBuildings.ForEach(building =>
        {
            if (building) building.Health.Die();
        });
        
        _createdBuildings.Clear();
    }
}