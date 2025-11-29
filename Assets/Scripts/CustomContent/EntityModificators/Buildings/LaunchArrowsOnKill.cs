using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Combat;
using System;

public sealed class LaunchArrowsOnKill : EntityModificator
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private Arrow _arrowPrefab;
    
    private float _delayBetweenShots;
    private float _projectileSpeed;
    private int _arrowsPerKill;
    private WeaponPool<Arrow> _arrowPool;
    
    public override void Enable()
    {
        _arrowPrefab = Args.GetArgument<GameObject>("ArrowPrefab").GetComponent<Arrow>();
        
        _delayBetweenShots = Args.GetArgument<float>("DelayBetweenShots");
        _projectileSpeed = Args.GetArgument<float>("ProjectileSpeed");
        _arrowsPerKill = Args.GetArgument<int>("ArrowsPerKill");

        _arrowPool = new WeaponPool<Arrow>(_arrowPrefab, 2, Entity, 5);
        foreach (Arrow arrow in _arrowPool.Pool) { SubscribeToArrow(arrow); }
        _arrowPool.PoolObject.ObjectCreated += SubscribeToArrow;

        Entity.DamageModifierContainer.EntityKilled += TryToLaunchArrowAsync;
    }

    private void TryToLaunchArrowAsync(CombatEntity _) => TryToLaunchArrow().Forget();
    private async UniTask TryToLaunchArrow()
    {
        for (int i = 0; i < _arrowsPerKill; i++)
        {
            if (Entity.ComponentsContainer.Get<AreaEntityDetector>().IsEmpty) return;

            Arrow arrow = _arrowPool.GetPooledWeapon();
        
            arrow.Launch(_projectileSpeed, Entity.ComponentsContainer.Get<AreaEntityDetector>().RandomItem.transform.position, Entity.transform.position);

            try
            {
                await UniTask.WaitForSeconds(_delayBetweenShots);
            }
            catch (Exception e)
            {
                e.LogAsync();
                return;
            }
        }
    }
    
    private void SubscribeToArrow(Arrow arrow) => arrow.OnArrowHit += OnArrowHit;
    private void OnArrowHit(Arrow arrow) => arrow.DisableArrow().Forget();
    
    public override void Disable()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        
        Entity.DamageModifierContainer.EntityKilled -= TryToLaunchArrowAsync;
        _arrowPool.DestroyPool();
    }
}