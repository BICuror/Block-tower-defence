using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;
using System;
using Combat;

public sealed class EnemyEgg : MonoBehaviour
{
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;

    [SerializeField] private float _invunrabilityPeriod = 0.5f;
    [SerializeField] private bool _autoAddToGlobalEnemyContainer;
    [SerializeField] private EnemyEntity _ownerEntity;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    
    private void Start()
    {
        if (_globalEnemyContainer.Entities.Count == 0)
        {
            _ownerEntity.Health.Die();
            return;
        }
        
        _ownerEntity.Health.InvulnerabilityTokenContainer.AddToken();
        _ownerEntity.Health.RefilHP();
        if (_autoAddToGlobalEnemyContainer) _globalEnemyContainer.Add(_ownerEntity); 
        
        if (_ownerEntity.ComponentsContainer.Get<Collider>().enabled) Debug.LogError("Egg collider should be disabled by default in prefab, otherwise it might break entity area detectors and global enemy list");
        
        _ownerEntity.ComponentsContainer.Get<Collider>().enabled = true;
    }

    public async UniTask TrySpawnEnemy(EnemyData enemyData)
    {
        await UniTask.WaitForSeconds(_invunrabilityPeriod);
        
        _ownerEntity.Health.InvulnerabilityTokenContainer.RemoveToken();
        
        try
        {
            await UniTask.WaitForSeconds(_ownerEntity.StatContainer.Get<SpawnDelay>().Value, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }

        EnemyFactory.Instance.CreateEnemy(enemyData, transform.position);
        
        _ownerEntity.Health.Die();
    }

    private void CancelSpawn()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose(); 
    }

    private void OnDestroy() => CancelSpawn();
}