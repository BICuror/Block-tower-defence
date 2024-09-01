using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class TextureManager
    { 
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private BiomeMapGenerator _biomeMap;

        public CubeTextures GetCubeTexture(Vector3Int position, BlockType type)
        {
            if (type == BlockType.Corruption) return _islandData.CorruptionBlock;
            else if (type == BlockType.CorruptionOnWater) return _islandData.CorruptionBlockOnWater;
            else if (type == BlockType.Road) return _islandData.RoadBlock;
            else if (type == BlockType.RoadOnWater) return _islandData.RoadBlockOnWater;
            
            IslandData.Biome biomeAtCurrentPosition = _biomeMap.GetBiomeAt(new Vector2Int(position.x, position.z));

            if (type == BlockType.Surface) return biomeAtCurrentPosition.SurfaceBiomBlock;
            return biomeAtCurrentPosition.RockBiomeBlock;
        }
    }
}

