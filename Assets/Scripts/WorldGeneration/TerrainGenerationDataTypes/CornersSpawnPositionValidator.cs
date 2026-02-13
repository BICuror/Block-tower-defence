using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "CornersSpawnPositionValidator", menuName = "Generation/SpawnerPositionValidators/CornersSpawnPositionValidator")]

    public class CornersSpawnPositionValidator : SpawnerPositionValidator
    {
        [SerializeField] private float _maxRangeFromCorners = 1.5f;

        public override bool IsValidPosition(int x, int maxX, int z, int maxZ, List<Vector2Int> exsistingBiomesIndexList)
        {
            return IsInCornder(x, maxX, z, maxZ);
        }

        private bool IsInCornder(int x, int maxX, int z, int maxZ)
        {
            Vector2Int closetCorner = GetClosestCorner(new Vector2Int(x, z), maxX, maxZ);
            
            return Vector2Int.Distance(closetCorner, new Vector2Int(x, z)) <= _maxRangeFromCorners;
        }

        private Vector2Int GetClosestCorner(Vector2Int index, int maxX, int maxZ)
        {
            Vector2Int closetCorner  = Vector2Int.zero;
            float minimalDistance = float.MaxValue;
            
            for (int x = 0; x <= 1; x++)
            {
                for (int z = 0; z <= 1; z++)
                {
                    float distance = Vector2Int.Distance(index, new Vector2Int(x * maxX, z * maxZ));

                    if (minimalDistance > distance)
                    {
                        closetCorner = new Vector2Int(x * maxX, z * maxZ);
                    }
                }
            }
            
            return closetCorner;
        }
    }
}
