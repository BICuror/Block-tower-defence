using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using Combat;

public sealed class ExplosionBehaviour : CombatBehaviour<Vector3>, IDisposable
{
    private Explotion _explosionPrefab;
    private WeaponPool<Explotion> _explosionPool;
        
    public override void Execute(Vector3 explotionPosition)
    {
        Explode(explotionPosition).Forget();
    }

    private async UniTask Explode(Vector3 explotionPosition)
    {
        if (_explosionPool == null) CreateNewPool();
        
        Explotion explotion = _explosionPool.GetPooledWeapon();
        
        explotion.transform.position = explotionPosition;
        await explotion.Explode();
        explotion.gameObject.SetActive(false);
    }

    private void CreateNewPool()
    {
        _explosionPool = new WeaponPool<Explotion>(Args.GetArgument<GameObject>("ExplosionPrefab").GetComponent<Explotion>(), 2, Entity);
    }
    
    public void Dispose()
    {
        _explosionPool.DestroyPool();
    }
}