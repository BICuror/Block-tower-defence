using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyHealth))]

public sealed class EnemyBootstrap : MonoBehaviour
{
    [SerializeField] private GPUInstancerEnabler _GPUInstancerEnabler;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;   

    [SerializeField] private NewEnemyNavAgent _navMeshAgent;
    public NavMeshAgent Agent;

    [SerializeField] private EnemyHealth _enemyHealth; 
    public EnemyHealth Health => _enemyHealth;

    [SerializeField] private Animator _animator;

    public void SetEnemyData(EnemyData enemyDataToSet)
    {
        SetEnemyHealthData(enemyDataToSet.HealthData);
        SetEnemyMovmentData(enemyDataToSet.MovmentData);
        SetVisualData(enemyDataToSet);
        CreateSpecialObject(enemyDataToSet);
    }

    private void SetEnemyMovmentData(EnemyMovmentData enemyData)
    {
        //_navMeshAgent.speed = enemyData.MovmentSpeed;
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
        
        //_navMeshAgent.SetDestination(EnemyDestanationSetter.Instance.GetFinalEnemyDestanation());
    }

    private void OnEnable() 
    {
        _navMeshAgent.enabled = false;

        _animator.Play("Entry");
    }
}
