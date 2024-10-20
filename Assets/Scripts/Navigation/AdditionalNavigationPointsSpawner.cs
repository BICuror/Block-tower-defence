using System.Collections.Generic;
using UnityEngine;
using Zenject;
using WorldGeneration;
using Navigation;

public sealed class AdditionalNavigationPointsGenerator : MonoBehaviour
{
    [Inject] private DiContainer _diContainer;
    [Inject] private IslandHeightMapHolder _heightMapHolder;
    [SerializeField] private GameObject _chest;
    [SerializeField] private int _minimalHeight = 1;

    public List<INavigationCondition> GeneratePointsAndConditions(List<Vector2Int> positionsToSpawn)
    {
        List<INavigationCondition> conditions = new();
        
        for (int i = 0; i < positionsToSpawn.Count; i++)
        {
            int height = _heightMapHolder.Map[positionsToSpawn[i].x, positionsToSpawn[i].y];

            if (height < _minimalHeight) height = _minimalHeight;

            GameObject chest = _diContainer.InstantiatePrefab(_chest, new Vector3(positionsToSpawn[i].x, height + 1f, positionsToSpawn[i].y), Quaternion.identity, null);
            
            ExsistanceNavigationCondition condition = new ExsistanceNavigationCondition(chest);
            
            conditions.Add(condition);
        }       

        return conditions;
    }
}