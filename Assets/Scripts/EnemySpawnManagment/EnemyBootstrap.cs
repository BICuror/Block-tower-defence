using System.Collections;
using System.Collections.Generic;
using Cashing;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Navigation;

namespace Combat
{
    [RequireComponent(typeof(EnemyHealth))]
    
    public sealed class EnemyBootstrap : MonoBehaviour
    {
        [SerializeField] private GPUInstancerEnabler _GPUInstancerEnabler;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;   
        [SerializeField] private Animator _animator;
        
        [Cached] private NavigationAgent _navMeshAgent;
        [Cached] private EnemyHealth _enemyHealth;
        [Cached] private StatContainer _statContainer;
        
        private EnemyData _enemyData;
    
        public void SetEnemyData(EnemyData enemyDataToSet)
        {
            _enemyData = enemyDataToSet;
            
            _statContainer.Get<MaxHealth>().SetDefault(enemyDataToSet.HealthData.MaxHealth);
            _statContainer.Get<Speed>().SetDefault(2f);
            
            _navMeshAgent.SetAgentData(enemyDataToSet.NavigationData);
            _enemyHealth.Initialize();
    
            SetVisualData(enemyDataToSet);
            //CreateSpecialObject(enemyDataToSet);
        }
    
        private void SetVisualData(EnemyData enemyData)
        {
            _meshFilter.sharedMesh = enemyData.GetMesh();
            _meshRenderer.sharedMaterial = enemyData.GetMaterial();
            _GPUInstancerEnabler.EnableGPUInstancing();
        }
    
        private void CreateSpecialObject(EnemyData enemyData)
        {
            //if (enemyData.GetSpecialObject() != null)
            //{
               // SpecialEnemyObject specialObject = Instantiate(enemyData.GetSpecialObject(), transform.position, transform.rotation, transform);
            
                //specialObject.SetEnemyHealth(_enemyHealth);
            //}
        }

        public async void StartNavigation() 
        {
            _animator.Play("Entry");

            await UniTask.WaitForSeconds(1f);
            
            _navMeshAgent.Initialize();
        }
    }
}