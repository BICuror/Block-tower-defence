using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class IslandTerrainMeshCreator : MonoBehaviour
    {
        [Inject] private TextureManager _textureManager;

        [SerializeField] private TerrainSetter _islandTerrainSetter;
        [SerializeField] private TerrainSetter _islandBottomTerrainSetter;

        public void CreateMesh(BlockGrid blockGrid)
        {
            IslandTerrainMeshGenerator meshGenerator = new IslandTerrainMeshGenerator();

            meshGenerator.SetupGenerator(blockGrid, _textureManager);

            _islandBottomTerrainSetter.SetMesh(meshGenerator.GetBottomMesh());
            
            _islandTerrainSetter.SetMesh(meshGenerator.GetDefaultMesh());
        }
    }
}