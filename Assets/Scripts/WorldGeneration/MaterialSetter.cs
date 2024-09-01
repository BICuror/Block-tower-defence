using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class MaterialSetter : MonoBehaviour
    {  
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        [SerializeField] private Material[] _enivromentMaterials;
        [SerializeField] private Material[] _buildingMaterials;
        [SerializeField] private Material[] _enemyDecorationsMaterials;

        private void Awake() => UpdateMaterialsTextures();

        public void UpdateMaterialsTextures()
        {
            SetEnivromentBaseMap();
            SetBuildingsBaseMap();
            SetEnemyDecorationsTextures();
        }

        public void SetEnivromentBaseMap()
        {
            for (int i = 0; i < _enivromentMaterials.Length; i++)
            {
                _enivromentMaterials[i].SetTexture("_BaseMap", _islandData.EniviromentTexture);
            }
        }

        public void SetBuildingsBaseMap()
        {
            for (int i = 0; i < _buildingMaterials.Length; i++)
            {
                _buildingMaterials[i].SetTexture("_BaseMap", _islandData.BuildingsTexture);
                _buildingMaterials[i].SetTexture("_EmissionMap", _islandData.BuildingsEmissionTexture);
            }
        }

        public void SetEnemyDecorationsTextures()
        {
            for (int i = 0; i < _enemyDecorationsMaterials.Length; i++)
            {
                _enemyDecorationsMaterials[i].SetTexture("_BaseMap", _islandData.EnemyBiomeDecorationsTextures);
            }
        }   
    }
}
