using Zenject;
using UnityEngine;
using System.Collections.Generic;

namespace WorldGeneration
{
    public sealed class SpawnerRotator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private RoadMapHolder _roadMapHolder;

        private Vector2Int[] _directions = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };

        public void RotateSpawner(Transform spawner)
        {
            Vector2Int spawnerPosition = new Vector2Int((int)spawner.position.x, (int)spawner.position.z);

            List<Vector2Int> roadPositions = new List<Vector2Int>();

            bool[,] roadMap = _roadMapHolder.Map;

            for (int i = 0; i < _directions.Length; i++)
            {
                Vector2Int currentPosition = _directions[i] + spawnerPosition;

                if (currentPosition.x >= 0 && currentPosition.x < _islandData.IslandSize && currentPosition.y >= 0 && currentPosition.y < _islandData.IslandSize)
                {
                    if (roadMap[currentPosition.x, currentPosition.y]) roadPositions.Add(_directions[i]);
                }
            }

            if (roadPositions.Count == 0) Debug.LogError("Road positions are not found nerby spawner");
            else if (roadPositions.Count == 1) RotateSpawnerToward(spawner, roadPositions[0]); 
            else 
            {
                Debug.LogError("Multiple road positions found nerby spawner");

                Vector2Int direction = spawnerPosition - new Vector2Int(_islandData.MiddleIndex, _islandData.MiddleIndex);

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) RotateSpawnerToward(spawner, new Vector2Int(GetNormalizedInt(direction.x), 0));
                else RotateSpawnerToward(spawner, new Vector2Int(0, GetNormalizedInt(direction.y)));
            }
        }

        private int GetNormalizedInt(int value)
        {
            if (value == 0) return 0;

            return value / Mathf.Abs(value);
        }
        
        private void RotateSpawnerToward(Transform spawner, Vector2Int position)
        {
            Vector3 lookPosition = spawner.position + new Vector3(position.x, 0f, position.y);

            spawner.LookAt(lookPosition);
        }
    }
}