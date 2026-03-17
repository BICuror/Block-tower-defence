using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public sealed class AOEBehaviour : CombatBehaviour<Vector3>, IDisposable
{
    private AOE _aoePrefab;
    private WeaponPool<AOE> _aoePool;

    public AOEBehaviour(ArgumentsContainer args)
    {
        Args = args;
    }
    
    public override void Execute(Vector3 explotionPosition)
    {
        Explode(explotionPosition).Forget();
    }

    private async UniTask Explode(Vector3 explotionPosition)
    {
        if (_aoePool == null) CreateNewPool();
        
        AOE aoe = _aoePool.GetPooledWeapon();
        
        aoe.transform.position = explotionPosition;
        await aoe.ActiveAOE();
    }

    private void CreateNewPool()
    {
        _aoePool = new WeaponPool<AOE>(Args.GetArgument<GameObject>("AOEPrefab").GetComponent<AOE>(), 2, Entity);
    }
    
    public void Dispose()
    {
        _aoePool.DestroyPool();
    }
}
