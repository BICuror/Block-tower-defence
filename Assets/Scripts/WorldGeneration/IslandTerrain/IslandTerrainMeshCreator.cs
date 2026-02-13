using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class IslandTerrainMeshCreator : MonoBehaviour
    {
        [Inject] private TextureManager _textureManager;

        [SerializeField] private TilemapOverlapSeamsGenerator _tilemapOverlapSeamsGenerator;
        [SerializeField] private IslandTileTerrainGenerator _islandTileTerrainGenerator;
        [SerializeField] private TerrainSetter _islandBottomTerrainSetter;
        [SerializeField] private TerrainSetter _islandTerrainSetter;
        [SerializeField] private MeshGenerator _meshGenerator;

        public void CreateMesh(BlockGrid blockGrid)
        {
            _meshGenerator.SetupGenerator(blockGrid, _textureManager);

            _islandBottomTerrainSetter.SetMesh(_meshGenerator.GetBottomMesh());
            
            _islandTerrainSetter.SetMesh(_meshGenerator.GetDefaultMesh());
            
            _islandTileTerrainGenerator.GenerateTerrain();
            
            _tilemapOverlapSeamsGenerator.GenerateTerrain();
        }
    }
}