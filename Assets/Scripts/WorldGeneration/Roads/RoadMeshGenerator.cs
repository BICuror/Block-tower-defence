using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public class RoadMeshGenerator : MeshGenerator
    {
        private BlockGrid _islandGrid;

        public void SetIslandGrid(BlockGrid grid) => _islandGrid = grid;

        protected override bool ShouldCheckBlock(Vector3Int positionToCheck) => true; //_islandGrid.GridSpaceIsEmpty(positionToCheck);
        
        protected override bool ShouldCheckThisBlockType(BlockType type)
        {
            return (type == BlockType.Road) || (type == BlockType.RoadOnWater);
        }
    }
}