using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class TextureManager
    { 
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        public CubeTextures GetCubeTexture()
        {
            return _islandData.DefaultTexture;
        }
    }
}

