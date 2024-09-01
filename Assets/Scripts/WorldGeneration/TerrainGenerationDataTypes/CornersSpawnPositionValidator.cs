using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "CornersSpawnPositionValidator", menuName = "Generation/SpawnerPositionValidators/CornersSpawnPositionValidator")]

    public class CornersSpawnPositionValidator : SpawnerPositionValidator
    {
        [SerializeField] private int _nodesStrength = 1;

        public override bool IsValidPosition(int x, int maxX, int z, int maxZ)
        {
            return (x == 0 || x == maxX - 1) && (z == 0 || z == maxZ - 1);
        }
    }
}
