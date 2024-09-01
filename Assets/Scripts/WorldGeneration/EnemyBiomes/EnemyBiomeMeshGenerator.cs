using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeMeshGenerator : MeshGenerator
    {
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private RoadGenerator _roadGenerator;

        private Vector3Int _position;

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
            return (type == BlockType.Corruption) || (type == BlockType.CorruptionOnWater);
        }
    }
}