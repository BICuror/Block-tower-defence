using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using Combat;

public sealed class ExplosionBehaviour : CombatBehaviour<Vector3>, IDisposable
{
    private Explosion _explosionPrefab;
    private WeaponPool<Explosion> _explosionPool;

    public ExplosionBehaviour(ArgumentsContainer args)
    {
        Args = args;
    }

    protected override void OnOwnerEntitySet()
    {
        Entity.StatContainer.AddStatIfDoesntExist<ExplosionRadius>(Args.GetArgument<float>("ExplosionRadius"));
        Entity.StatContainer.AddStatIfDoesntExist<ExplosionDamage>(Args.GetArgument<float>("ExplosionDamage"));
    }
        
    public override void Execute(Vector3 explotionPosition)
    {
        Explode(explotionPosition).Forget();
    }

    private async UniTask Explode(Vector3 explotionPosition)
    {
        if (_explosionPool == null) CreateNewPool();
        
        Explosion explosion = _explosionPool.GetPooledWeapon();
        
        explosion.transform.position = explotionPosition;
        await explosion.Explode();
        explosion.gameObject.SetActive(false);
    }

    private void CreateNewPool()
    {
        _explosionPool = new WeaponPool<Explosion>(Args.GetArgument<GameObject>("ExplosionPrefab").GetComponent<Explosion>(), 2, Entity);
    }
    
    public void Dispose()
    {
        _explosionPool.DestroyPool();
    }
}