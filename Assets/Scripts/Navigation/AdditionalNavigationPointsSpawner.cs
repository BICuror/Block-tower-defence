using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using WorldGeneration;

namespace Navigation
{
    public sealed class AdditionalNavigationPointsSpawner : MonoBehaviour
    {
        [Inject] private IslandHeightMapHolder _heightMapHolder;
        [Inject] private NavigationMapHolder _navigationMap;
        [SerializeField] private GameObject Chest;
        [Range(0f, 1f)] [SerializeField] private float _chanceToSpawnAdditionalNavPointPerEnemyBiome = 0.5f;


        [SerializeField] private DefaultNavigationMapGenerator _def;
        [SerializeField] private OptionalNavigationMapGenerator _op;

        public void SpawnAdditionalNavigationPoints()
        {
            
        }

        public void GeneratePointsPositions(List<Vector2Int> positionsToSpawn, List<Vector2Int> spawnPosition)
        {
            List<INavigationCondition> conditions = new();
            for (int i = 0; i < positionsToSpawn.Count; i++)
            {
                int height = _heightMapHolder.Map[positionsToSpawn[i].x, positionsToSpawn[i].y] + 1;
                if (height < 2) height = 2;
                GameObject chest = Instantiate(Chest, new Vector3(positionsToSpawn[i].x, height, positionsToSpawn[i].y), Quaternion.identity);
                ExsistanceNavigationCondition condition = new ExsistanceNavigationCondition(chest);
                conditions.Add(condition);
            }       

            NavigationMap navMap = new NavigationMap(_heightMapHolder.Map.GetLength(0));

            _navigationMap.SetNavigationMap(navMap);

            _def.GenerateDefaultNodeMap();
            _op.GenerateOptionalNodeMap(positionsToSpawn, conditions, spawnPosition);
        }
    }
}

