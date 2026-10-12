using WorldGeneration;
using UnityEngine;
using Navigation;
using Zenject;

namespace Combat.OptionalTasks
{
    public abstract class OptionalTaskGenerator : MonoBehaviour
    {
        [Inject] private OptionalTaskObjectsManager _optionalTaskObjectsManager;
        [Inject] private GlobalBuildingContainer _globalBuildingContainer;
        [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
        [Inject] private DiContainer _diContainer;
        
        public abstract bool TryGenerateOptionalTask(Vector2Int spawnerPosition, out AdditionalTaskLayerPrebuildData layerPrebuildData, out OptionalTask optionalTask);

        protected OptionalTask SpawnOptionalTask(OptionalTask optionalTaskPrefab, Vector2Int spawnPosition)
        {
            int spawnHeight = _islandHeightMapHolder.GetHeightSafe(spawnPosition) + 1;

            if (spawnHeight < 2) spawnHeight = 2;
            
            OptionalTask taskObject = _diContainer.InstantiatePrefab(optionalTaskPrefab, spawnPosition.ToVector3(spawnHeight), Quaternion.identity, null).GetComponent<OptionalTask>();
            _globalBuildingContainer.Add(taskObject.GetComponent<BuildingEntity>());
            _optionalTaskObjectsManager.Add(taskObject);
            
            return taskObject;
        }
    }
}