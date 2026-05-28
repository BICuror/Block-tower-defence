using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public abstract class DecorationGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        
        [SerializeField] private bool _generateWaterDecorations;
        [SerializeField] private DecorationContainer _decorationContainer;

        public void GenerateDecorations(BlockGrid blockGrid, Vector2 offset)
        {
            int areaSize = blockGrid.GetSize();
        
            _decorationContainer.CreateNewContainer();

            int halfAreaSize = Mathf.RoundToInt(areaSize / 2f);
            
            for (int x = -halfAreaSize; x < areaSize + halfAreaSize; x++)
            {
                for (int z = -halfAreaSize; z < areaSize + halfAreaSize; z++)
                {
                    if (Vector2Int.Distance(new Vector2Int(x, z), new Vector2Int(halfAreaSize, halfAreaSize)) > areaSize) continue;

                    DecorationModule decorationModule;
                    
                    Vector3Int spawnPosition = new Vector3Int(x, 0, z);
                    
                    if (blockGrid.IsInBounds(spawnPosition) && blockGrid.GetMaxHeight(x, z) > 0)
                    {
                        decorationModule = GetDecorationModule(x, z);

                        spawnPosition.y = blockGrid.GetMaxHeight(x, z);
                    }
                    else if (_generateWaterDecorations)
                    {
                        decorationModule = _islandDataContainer.Data.WaterDecorationsModule;
                    }
                    else continue;
                    
                    if (decorationModule.DecorationAppearRate > Random.Range(0f, 1f))
                    {
                        DecorationData decoration = GetRandomDecoration(decorationModule.Decorations);

                        Vector2Int checkPosition = new Vector2Int(x + Mathf.RoundToInt(offset.x), z + Mathf.RoundToInt(offset.y));
                        
                        if (_decorationContainer.GetHightestPossibleDecorationRadius(checkPosition) > decoration.Prefabs[0].TileRadius)
                        {
                            CreateDecorations(decoration, spawnPosition, offset);
                        }
                    }
                }
            }
        }

        protected abstract DecorationModule GetDecorationModule(int x, int z);

        private void CreateDecorations(DecorationData decoration, Vector3Int position, Vector2 offset)
        {
            int amountOfDecorations = Random.Range(1, decoration.Amount); 

            for (int i = 0; i < amountOfDecorations; i++)
            {
                Vector3 spawnPosition = position + new Vector3(Random.Range(-decoration.PlacementOffset, decoration.PlacementOffset), 0.5f, Random.Range(-decoration.PlacementOffset, decoration.PlacementOffset));

                float randomXRot = GetRandomRotationAngle(decoration.RotateXAxis, decoration.LockToRightAngleRotation);
                float randomYRot = GetRandomRotationAngle(decoration.RotateYAxis, decoration.LockToRightAngleRotation);
                float randomZRot = GetRandomRotationAngle(decoration.RotateZAxis, decoration.LockToRightAngleRotation);
                
                Quaternion rotation = Quaternion.Euler(randomXRot, randomYRot, randomZRot);
                
                DecorationObject decorationObject = Instantiate(decoration.Prefabs[Random.Range(0, decoration.Prefabs.Length)], spawnPosition + new Vector3(offset.x, 0f, offset.y), rotation, transform);

                float randomScale = Random.Range(decoration.MinScale, decoration.MaxScale);
                Vector3 scale = new Vector3(randomScale, randomScale, randomScale); 
                
                if (decoration.HasYScale) scale.y = Random.Range(decoration.MinYScale, decoration.MaxYScale);
                
                decorationObject.transform.localScale = scale; 
                decorationObject.SetDefaultScale(scale);
                decorationObject.SetState(true);
                
                _decorationContainer.AddDecorations(position.x, position.z, decorationObject);
            }
        }

        private DecorationData GetRandomDecoration(Decoration[] decorations)
        {
            float random = Random.Range(0f, 1f);

            for (int i = 0; i < decorations.Length; i++)
            {
                if (random < decorations[i].AppearRate) return decorations[i].DecorationData;
            }

            Debug.LogError("DecorationObject not found");
            return new DecorationData();
        }

        private float GetRandomRotationAngle(bool rotationAllowed, bool lockToRightAngle)
        {
            if (!rotationAllowed) return 0f;
            
            if (!lockToRightAngle) return Random.Range(0f, -360f);
                
            return Random.Range(0, 4) * 90f;
        }
    }
}
