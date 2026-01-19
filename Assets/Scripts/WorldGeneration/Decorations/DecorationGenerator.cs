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
        
            _decorationContainer.CreateNewContainer(areaSize);

            for (int x = 0; x < areaSize; x++)
            {
                for (int z = 0; z < areaSize; z++)
                {
                    if (blockGrid.GetMaxHeight(x, z) > 0)
                    {
                        DecorationModule decorationModule = GetDecorationModule(x, z);
    
                        if (decorationModule.DecorationAppearRate > Random.Range(0f, 1f))
                        {         
                            CreateDecorations(GetRandomDecoration(decorationModule.Decorations), new Vector3Int(x, blockGrid.GetMaxHeight(x, z), z), offset);
                        }
                    }
                    else if (_generateWaterDecorations)
                    {
                        DecorationModule decorationModule = _islandDataContainer.Data.WaterDecorationsModule;
                        
                        if (decorationModule.DecorationAppearRate > Random.Range(0f, 1f))
                        {         
                            CreateDecorations(GetRandomDecoration(decorationModule.Decorations), new Vector3Int(x, blockGrid.GetMaxHeight(x, z), z), offset);
                        }
                    }
                }
            }
        }

        protected abstract DecorationModule GetDecorationModule(int x, int z);

        private void CreateDecorations(DecorationData decoration, Vector3Int position, Vector2 offset)
        {
            int amountOfDecorations = Random.Range(1, decoration.Amount); 

            MeshRenderer[] decorations = new MeshRenderer[amountOfDecorations];

            for (int i = 0; i < amountOfDecorations; i++)
            {
                Vector3 spawnPosition = position + new Vector3(Random.Range(-decoration.PlacmentOffset, decoration.PlacmentOffset), 0.5f, Random.Range(-decoration.PlacmentOffset, decoration.PlacmentOffset));

                float randomXRot = Random.Range(-decoration.MaxRotation.x, decoration.MaxRotation.x);    
                float randomYRot = Random.Range(-decoration.MaxRotation.y, decoration.MaxRotation.y); 
                float randomZRot = Random.Range(-decoration.MaxRotation.z, decoration.MaxRotation.z); 
                
                decorations[i] = Instantiate(decoration.Prefabs[Random.Range(0, decoration.Prefabs.Length)], spawnPosition + new Vector3(offset.x, 0f, offset.y), Quaternion.Euler(randomXRot, randomYRot, randomZRot), transform);                    

                float randomScale = Random.Range(decoration.MinScale, decoration.MaxScale);
                decorations[i].transform.localScale = new Vector3(randomScale, randomScale, randomScale); 
            }

            _decorationContainer.AddDecorations(position.x, position.z, decorations);
        }

        private DecorationData GetRandomDecoration(Decoration[] decorations)
        {
            float random = Random.Range(0f, 1f);

            for (int i = 0; i < decorations.Length; i++)
            {
                if (random < decorations[i].AppearRate) return decorations[i].DecorationData;
            }

            Debug.LogError("Decoration not found");
            return new DecorationData();
        }
    }
}
