using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeMapGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        public bool[,] GenerateEnemyMap(int biomeStage)
        {
            int radius = _islandData.EnemyBiomeStages[biomeStage].EnemyBiomeRadius;

            AnimationCurve biomeCurve = _islandData.EnemyBiomeStages[biomeStage].EnemyBiomeEdgeReductionCurve;

            bool[,] enemyBiomeMap = new bool[radius * 2 + 1, radius * 2 + 1];

            for (int x = 0; x < radius * 2 + 1; x++)
            {
                for (int y = 0; y < radius * 2 + 1; y++)
                {
                    int distance = Mathf.Abs(radius + 1 - x) + Mathf.Abs(radius + 1 - y);

                    enemyBiomeMap[x, y] = (Random.Range(0f, 1f) > biomeCurve.Evaluate(Mathf.Lerp(0, 1, distance / radius)));                    
                }
            }

            return enemyBiomeMap;
        }
    }
}