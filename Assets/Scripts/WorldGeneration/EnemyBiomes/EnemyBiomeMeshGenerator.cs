using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeMeshGenerator : MeshGenerator
    {
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private RoadGenerator _roadGenerator;

        private Vector3Int _position;
        
        protected override float BottomWallHeight => Random.Range(0.5f, 3f);
        
        public void SetPosition(Vector3Int position) => _position = position;

        protected override bool ShouldCheckBlock(Vector3Int positionToCheck) 
        {
            Vector3Int checkPosition = positionToCheck + _position;

            if (_islandGridHolder.Grid.IsInBounds(checkPosition))
            {
                if (_islandGridHolder.Grid.GridSpaceIsEmpty(checkPosition) == false) return false;
            }

            if (_roadGenerator.RoadGrid.IsInBounds(checkPosition))
            {
                if (_roadGenerator.RoadGrid.GridSpaceIsEmpty(checkPosition) == false) return false;
            }

            return true;
        }  
    }
}