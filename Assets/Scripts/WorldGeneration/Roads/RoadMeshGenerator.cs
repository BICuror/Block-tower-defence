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

        protected override void ApplyTextureToFace(Vector3Int position, Vector3Int checkDirection, BlockType blockType, FaceData faceToApply)
        {
            CubeTextures cubeTextures = _textureManager.GetCubeTexture(position, blockType);

            Vector2[] UVsToAdd = cubeTextures.GetUVsAtDirection(checkDirection);

            for (int i = 0; i < faceToApply.UVOrder.Length; i++)
            {
                UVs.Add(UVsToAdd[faceToApply.UVOrder[i]]);
            }
        }

        protected override bool ShouldCheckThisBlockType(BlockType type)
        {
            return (type == BlockType.Road) || (type == BlockType.RoadOnWater);
        }
    }
}