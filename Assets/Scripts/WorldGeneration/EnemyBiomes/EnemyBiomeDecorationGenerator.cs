using UnityEngine;

namespace WorldGeneration
{
    public sealed class EnemyBiomeDecorationGenerator : DecorationGenerator
    {
        private IslandData.DecorationModule _decorationModule;

        public void SetDecorationModule(IslandData.DecorationModule decorationModule) => _decorationModule = decorationModule; 

        protected override IslandData.DecorationModule GetDecorationModule(int x, int z) => _decorationModule;
    }
}