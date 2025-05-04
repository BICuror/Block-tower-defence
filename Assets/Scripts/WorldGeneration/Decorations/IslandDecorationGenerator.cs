using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class IslandDecorationGenerator : DecorationGenerator
    {
        [Inject] private BiomeMapGenerator _biomeMapGenerator;

        protected override DecorationModule GetDecorationModule(int x, int z) 
        {
            return _biomeMapGenerator.GetBiomeAt(new Vector2Int(x, z)).DecorationsModule;
        }
    }
}
