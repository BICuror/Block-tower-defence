using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "EdgeSpawnerPositionValidator", menuName = "Generation/SpawnerPositionValidators/EdgeSpawnerPositionValidator")]

    public class EdgeSpawnerPositionValidator : SpawnerPositionValidator
    {
        public override bool IsValidPosition(int x, int maxX, int z, int maxZ, List<Vector2Int> exsistingBiomesIndexList)
        {
            return (x == 0 || x == maxX - 1) || (z == 0 || z == maxZ - 1) && ((x != (maxX - 1) / 2) || (z != (maxZ - 1) / 2));
        }
    }
}