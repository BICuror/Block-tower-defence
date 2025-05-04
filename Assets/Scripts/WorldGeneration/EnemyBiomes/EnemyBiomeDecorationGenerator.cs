using UnityEngine;

namespace WorldGeneration
{
    public sealed class EnemyBiomeDecorationGenerator : DecorationGenerator
    {
        private DecorationModule _decorationModule;

        public void SetDecorationModule(DecorationModule decorationModule) => _decorationModule = decorationModule; 

        protected override DecorationModule GetDecorationModule(int x, int z) => _decorationModule;
    }
}