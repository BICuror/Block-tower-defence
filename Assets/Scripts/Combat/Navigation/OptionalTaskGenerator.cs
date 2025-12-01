using UnityEngine;
using Navigation;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using WorldGeneration;

public sealed class OptionalTaskGenerator : MonoBehaviour
{
    [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
    [SerializeField] private AdditionalNavigationPointsPositionGenerator _additionalNavigationPointsPositionGenerator;
    [SerializeField] private AdditionalNavigationPointsGenerator _additionalNavigationPointsSpawner;

    private List<Vector2Int> _taskPositions = new();
    private List<INavigationCondition> _conditions = new();

    public List<Vector2Int>  TaskPositions => _taskPositions;
    public List<INavigationCondition> Conditions => _conditions;

    public void GenerateTasksAndModifyRoadMap()
    {
        List<Vector2Int> spawnerPositions = _enemyBiomeContainer.GetEnemyBiomesPositions();
        
        _taskPositions = _additionalNavigationPointsPositionGenerator.GeneratePositionsAndConnectToRoad(spawnerPositions);

        _conditions = _additionalNavigationPointsSpawner.GeneratePointsAndConditions(_taskPositions);
    }   
}