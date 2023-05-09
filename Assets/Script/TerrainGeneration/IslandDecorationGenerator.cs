using UnityEngine;

public class IslandDecorationGenerator : DecorationGenerator
{
    private BiomeMapGenerator _biomeMapGenerator;

    public void SetBiomeMap(BiomeMapGenerator biomeMapGenerator) => _biomeMapGenerator = biomeMapGenerator;

    protected override IslandData.DecorationModule GetDecorationModule(int x, int z) => _biomeMapGenerator.GetBiomeAt(new Vector2Int(x, z)).DecorationsModule;
}
