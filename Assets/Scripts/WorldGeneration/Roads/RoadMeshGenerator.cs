using UnityEngine;

namespace WorldGeneration
{
    public class RoadMeshGenerator : MeshGenerator
    {
        protected override float BottomWallHeight => Random.Range(0.25f, 2f);

        protected override bool ShouldCheckBlock(Vector3Int positionToCheck) => true;
        
        protected override bool ShouldCheckThisBlockType(BlockType type)
        {
            return (type == BlockType.Road) || (type == BlockType.RoadOnWater);
        }
    }
}