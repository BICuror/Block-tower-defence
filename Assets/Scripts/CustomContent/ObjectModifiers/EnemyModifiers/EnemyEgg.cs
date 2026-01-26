using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using Zenject;
using System;
using Combat;

public sealed class EnemyEgg : MonoBehaviour
{
    [SerializeField] private bool _autoAddToGlobalEnemyContainer;
    
    [Inject] private GlobalEnemyContainer _globalEnemyContainer;
    [Cached] private EnemyEntity _ownerEntity;
    [Cached] private SpawnDelay _spawnDelay;
    
    private CancellationTokenSource _cancellationTokenSource = new();

    private void Start()
    {
        if (_globalEnemyContainer.Entities.Count == 0)
        {
            _ownerEntity.Health.Die();
        }
        else
        {
            if (_autoAddToGlobalEnemyContainer) _globalEnemyContainer.Add(_ownerEntity);
            _ownerEntity.Health.Died += CancelSpawn;
            _ownerEntity.Health.RefilHP();
        }
    }

    public async UniTask TrySpawnEnemy(EnemyData enemyData)
    {
        try
        {
            await UniTask.WaitForSeconds(3, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }

        if (_globalEnemyContainer.Entities.Count > 1 || _globalEnemyContainer.Entities[0] != _ownerEntity)
        {
            EnemyFactory.Instance.CreateEnemy(enemyData, transform.position);
        }
        
        _ownerEntity.Health.Die();
    }

    private void CancelSpawn()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose(); 
        _cancellationTokenSource = new();
    }

    private void OnDestroy()
    {
        _ownerEntity.Health.Died -= CancelSpawn;
    }
}