using System.Runtime.InteropServices;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;
using Random = UnityEngine.Random;

public sealed class DraggableCreator : MonoBehaviour
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private DiContainer _diContainer;

    [Header("SpawnPositionSettings")]
    [SerializeField] private GameObject _draggableBlocker;
    [SerializeField] private LayerSetting _terrainLayerSettings;    
    [SerializeField] private LayerSetting _solidObjectsLayerSettings;   
    [Range(1f, 5f)] [SerializeField] private int _spawnRadius;
    [SerializeField] private Launcher _defaultLauncherPrefab;
    
    public async UniTask<DraggableObject> CreateDraggableOnPosition(DraggableObject draggablePrefab, Vector3 centerPosition, Vector3 finalPosition, [Optional]Launcher launcherPrefab)
    {
        if (!launcherPrefab) launcherPrefab = _defaultLauncherPrefab;

        DraggableObject createdDraggable = _diContainer.InstantiatePrefab(draggablePrefab, finalPosition, Quaternion.identity, null).GetComponent<DraggableObject>();

        await LaunchDraggable(createdDraggable, centerPosition, finalPosition, launcherPrefab);
        
        return createdDraggable;
    }

    public async UniTask LaunchDraggable(DraggableObject draggable, Vector3 centerPosition, Vector3 finalPosition, [Optional] Launcher launcherPrefab)
    {
        if (!launcherPrefab) launcherPrefab = _defaultLauncherPrefab;
        
        draggable.gameObject.SetActive(false);
        
        await CreateLauncher(centerPosition, finalPosition, launcherPrefab);

        TryToAddToGlobalBuildingContainer(draggable);
        
        draggable.transform.position = finalPosition;
        draggable.gameObject.SetActive(true);
    }
    
    public async UniTask<DraggableObject> CreateDraggableOnNearbyPosition(DraggableObject draggablePrefab, Vector3 startPositon, Vector3 finalPosition, [Optional]Launcher launcherPrefab)
    {
        if (!launcherPrefab) launcherPrefab = _defaultLauncherPrefab;

        finalPosition = GetRandomSpawnPosition(draggablePrefab, finalPosition, 0);

        return await CreateDraggableOnPosition(draggablePrefab, startPositon, finalPosition, launcherPrefab);
    }
    
    public async UniTask<DraggableObject> CreateDraggableOnRandomPosition(DraggableObject draggablePrefab, Vector3 startPositon, [Optional]int radius, [Optional]Launcher launcherPrefab)
    {
        if (radius == 0) radius = _spawnRadius;
        if (!launcherPrefab) launcherPrefab = _defaultLauncherPrefab;

        Vector3 finalPosition = GetRandomSpawnPosition(draggablePrefab, startPositon, radius);

        return await CreateDraggableOnPosition(draggablePrefab, startPositon, finalPosition, launcherPrefab);
    }

    public async UniTask ActivateDraggableOnRandomPosition(DraggableObject draggable, Vector3 startPositon, [Optional] int radius, [Optional] Launcher launcherPrefab)
    {
        if (radius == 0) radius = _spawnRadius;
        if (!launcherPrefab) launcherPrefab = _defaultLauncherPrefab;
        
        Vector3 finalPosition = GetRandomSpawnPosition(draggable, startPositon, radius);
        await CreateLauncher(startPositon, finalPosition, launcherPrefab);

        draggable.transform.position = finalPosition;
        draggable.gameObject.SetActive(true);
    }
    
    private async UniTask CreateLauncher(Vector3 startPosition, Vector3 finalPosition, Launcher launcherPrefab)
    {
        GameObject draggableBlocker = Instantiate(_draggableBlocker, finalPosition, Quaternion.identity);
        
        Launcher launcher = Instantiate(launcherPrefab, startPosition, Quaternion.identity);
        
        await launcher.Launch(startPosition, finalPosition);
        
        Destroy(draggableBlocker);
    }

    private void TryToAddToGlobalBuildingContainer(DraggableObject draggableObject)
    {
        if (draggableObject.TryGetComponent(out BuildingEntity buildingEntity))
        {
            _globalBuildingContainer.Add(buildingEntity);
        }
    }

    #region SpawnPositionPicking
    private Vector3 GetRandomSpawnPosition(DraggableObject draggableObject, Vector3 centerPosition, int radius)
    {
        Vector2Int roundedCenterPosition = new Vector2Int(Mathf.RoundToInt(centerPosition.x), Mathf.RoundToInt(centerPosition.z));
        
        List<Vector2Int> foundPositions = TileMap.FindClosestValidPositionsInRadius(IsSuitablePosition, roundedCenterPosition, radius, _islandDataContainer.Data.IslandSize);
        
        Vector2 selectedPosition = foundPositions[Random.Range(0, foundPositions.Count)];

        selectedPosition += TileMap.GetTileSizeDraggableObjectOffset(draggableObject.TileScale);
        
        selectedPosition = draggableObject.GetPlacementModule().GetPlacementPosition(selectedPosition, draggableObject.TileScale);
        
        float height = draggableObject.GetPlacementModule().GetHeight(selectedPosition);

        return new Vector3(selectedPosition.x, height, selectedPosition.y);

        bool IsSuitablePosition(Vector2Int position)
        {
            if (draggableObject.TileScale % 2 == 0) return draggableObject.GetPlacementModule().CanBePlaced(position + TileMap.GetTileSizeDraggableObjectOffset(draggableObject.TileScale), draggableObject.TileScale);
            
            return draggableObject.GetPlacementModule().CanBePlaced(position, draggableObject.TileScale);
        }
    }
    #endregion
}