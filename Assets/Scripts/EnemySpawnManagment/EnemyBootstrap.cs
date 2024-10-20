using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

[RequireComponent(typeof(EnemyHealth))]

public sealed class EnemyBootstrap : MonoBehaviour
{
    [SerializeField] private GPUInstancerEnabler _GPUInstancerEnabler;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;   

    [SerializeField] private NavigationAgent _navMeshAgent;
    public NavigationAgent Agent;

    [SerializeField] private EnemyHealth _enemyHealth; 
    public EnemyHealth Health => _enemyHealth;

    [SerializeField] private Animator _animator;

    private EnemyData _enemyData;

    public void SetEnemyData(EnemyData enemyDataToSet)
    {
        _enemyData = enemyDataToSet;

        SetEnemyHealthData(enemyDataToSet.HealthData);
        SetVisualData(enemyDataToSet);
        CreateSpecialObject(enemyDataToSet);
    }

    private void SetEnemyMovmentData(NavigationAgentData enemyData)
    {
        _navMeshAgent.Init(enemyData);
    }    

    private void SetEnemyHealthData(EnemyHealthData enemyData)
    {
        _enemyHealth.SetEnemyData(enemyData);
    }    

    private void SetVisualData(EnemyData enemyData)
    {
        _meshFilter.sharedMesh = enemyData.GetMesh();
        _meshRenderer.sharedMaterial = enemyData.GetMaterial();
        _GPUInstancerEnabler.EnableGPUInstancing();
    }

    private void CreateSpecialObject(EnemyData enemyData)
    {
        if (enemyData.GetSpecialObject() != null)
        {
            SpecialEnemyObject specialObject = Instantiate(enemyData.GetSpecialObject(), transform.position, transform.rotation, transform);
        
            specialObject.SetEnemyHealth(_enemyHealth);
        }
    }

    public void EnableNavmeshAgent()
    {
        _navMeshAgent.enabled = true;
        SetEnemyMovmentData(_enemyData.NavigationData);
    }

    private void OnEnable() 
    {
        _navMeshAgent.enabled = false;

        _animator.Play("Entry");
    }
}
