using UnityEngine;

public class TextureManager : MonoBehaviour
{ 
    private BiomeMapGenerator _biomeMap;

    public void SetBiomeMap(BiomeMapGenerator biomeMap) => _biomeMap = biomeMap;

    public CubeTextures GetCubeTexture(Vector3Int position, BlockType type)
    {
        if (type == BlockType.Corruption) return IslandDataContainer.GetData().CorruptionBlock;
        else if (type == BlockType.Road) return IslandDataContainer.GetData().RoadBlock;
        
        IslandData.Biome biomeAtCurrentPosition = _biomeMap.GetBiomeAt(new Vector2Int(position.x, position.z));

        if (type == BlockType.Surface) return biomeAtCurrentPosition.SurfaceBiomBlock;
        return biomeAtCurrentPosition.RockBiomeBlock;
    }
}
