using Cysharp.Threading.Tasks;
using UnityEngine;
using EventLinks;
using Configs;
using System;
using Combat;

public sealed class ExpandableBurstSubDomain : SubDomain
{
    [Argument("ExpandableBurstPrefab")] private GameObject _expandableBurstPrefab;
    [Argument("BurstRadius (0 for reach area scale)")] private float _burstRadius;

    private WeaponPool<ExpandableBurst> _burstPool;
    
    public event Action<CombatEntity> OnEntityHit;

    public void SpawnBurst(Vector3 position)
    {
        ExpandableBurst burst = _burstPool.GetPooledWeapon();
        burst.transform.position = position;

        burst.EnableAndExpand().Forget();
    }
    
    private void OnBurstCreation(ExpandableBurst burst)
    {
        burst.HitEntity += InvokeOnEntityHit;
    }
    
    private void InvokeOnEntityHit(CombatEntity entity) => OnEntityHit?.Invoke(entity);
    
    public override UniTask Enable()
    {
        _burstPool = new WeaponPool<ExpandableBurst>(_expandableBurstPrefab.GetComponent<ExpandableBurst>(), 3, Args.Owner);
        
        _burstPool.SetWeaponCreatedDelegate(OnBurstCreation);

        return UniTask.CompletedTask;
    }

    public override UniTask Disable()
    {
        _burstPool.DestroyPool();
        
        return UniTask.CompletedTask;
    }
}

public sealed class SpawnExpandableBurstCommand : SubDomainCommand<ExpandableBurstSubDomain>, ICommand
{
    public UniTask Execute()
    {
        SubDomain.SpawnBurst(Args.Owner.transform.position);
        
        return UniTask.CompletedTask;
    }
}

public sealed class ExpandableBurstHit : EventLinkWithParameter<CombatEntity>
{
    protected override void SubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Domain.GetSubDomain<ExpandableBurstSubDomain>().OnEntityHit += action;
    }

    protected override void UnsubscribeToEvent(Action<CombatEntity> action)
    {
        Args.Domain.GetSubDomain<ExpandableBurstSubDomain>().OnEntityHit -= action;
    }
}