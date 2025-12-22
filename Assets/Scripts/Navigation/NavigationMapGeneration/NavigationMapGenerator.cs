using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Navigation
{
    public class NavigationMapGenerator : MonoBehaviour
    {
        [Inject] private NavigationNodeMapGenerator _navigationNodeMapGenerator;
        [Inject] private OptionalTaskManager _optionalTaskManager;
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private NavigationMapper _navigationMapper;

        public void GenerateMap()
        {
            ResetNavigationMap();
            
            _navigationNodeMapGenerator.GenerateNodeMap();

            GenerateMainNavigationLayer();
            GenerateOptionalNavigationLayer();
        }
        
        private void ResetNavigationMap()
        {
            _navigationMapHolder.Map.ResetNodeMap();
            _navigationMapHolder.Map.ClearAllLayers();
        }

        private void GenerateMainNavigationLayer()
        {
            int centerPosition = _islandDataContainer.Data.IslandSize / 2;

            CreateLayer(new Vector2Int(centerPosition, centerPosition), NavigationMapLayerType.Main);
        }
        
        private void GenerateOptionalNavigationLayer()
        {
            for (int i = 0; i < _optionalTaskManager.LayerPrebuildDatas.Count; i++)
            {
                CreateLayer(_optionalTaskManager.LayerPrebuildDatas[i].Position, NavigationMapLayerType.AdditionalTask).SetNavigationCondition(_optionalTaskManager.LayerPrebuildDatas[i].NavigationCondition);
            }
        }

        private NavigationMapLayer CreateLayer(Vector2Int position, NavigationMapLayerType layerType)
        {
            Dictionary<Vector2Int, int> nodeWeights = _navigationMapper.GenerateNavigationLayerWeights(position);

            NavigationMapLayer navigationMapLayer = _navigationMapHolder.Map.CreateLayerAndAdd(nodeWeights, layerType);

            if (_islandDataContainer.Data.CenterShouldBeFlat)
            {
                SetCenterNodeWeightToAllCenter(navigationMapLayer);
            }
            
            return navigationMapLayer;
        }

        private void SetCenterNodeWeightToAllCenter(NavigationMapLayer navigationMapLayer)
        {
            int centerPositionIndex = _islandDataContainer.Data.CenterPositionIndex;
            int centerFlatRadius = 1;

            for (int x = centerPositionIndex - centerFlatRadius; x <= centerPositionIndex + centerFlatRadius; x++)
            {
                for (int z = centerPositionIndex - centerFlatRadius; z <= centerPositionIndex + centerFlatRadius; z++)
                {
                    navigationMapLayer.SetNodeWeight(new Vector2Int(x, z), 0);
                }
            }
        }
    }

    public sealed class AdditionalTaskLayerPrebuildData
    {
        private INavigationCondition _navigationCondition;
        private Vector2Int _position;
        
        public AdditionalTaskLayerPrebuildData(INavigationCondition navigationCondition, Vector2Int position)
        {
            _navigationCondition = navigationCondition;
            _position = position;
        }
        
        public INavigationCondition NavigationCondition => _navigationCondition;
        public Vector2Int Position => _position;
    }
}
