using UnityEngine;
using Zenject;
using WorldGeneration;

namespace Navigation
{
    public class NavigationMapGenerator : MonoBehaviour
    {
        [Inject] private DefaultNavigationMapGenerator _defaultNavigationMapGenerator; 
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private OptionalNavigationMapGenerator _optionalNavigationMapGenerator;
        [Inject] private NavigationMapHolder _navigationMapHolder;

        public void GenerateMap()
        {
            ResetNavigationMap();

            _defaultNavigationMapGenerator.GenerateDefaultNodeMap();
        
            _optionalNavigationMapGenerator.GenerateOptionalNodeMap();
        }

        private void ResetNavigationMap()
        {
            NavigationMap navMap = new NavigationMap(_islandDataContainer.Data.IslandSize);
            _navigationMapHolder.SetNavigationMap(navMap);
        }
    }
}
