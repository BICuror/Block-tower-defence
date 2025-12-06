using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Combat;
using System;

public sealed class Bomb : WeaponBase
{  
    [SerializeField] private VisualEffectHandler _fuseVisualEffect;
    [SerializeField] private DraggableObject _draggableObject;
    [SerializeField] private Explosion _explosion;
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _canBeExploded;
    private bool _isFree = true;

    public bool IsFree => _isFree;
    public DraggableObject DraggableObject => _draggableObject;
    
    public Action<Bomb> Exploded;
    
    protected override void OnInitialized()
    {
        _explosion.Initialize(OwnerEntity);
        
        _draggableObject.Placed += StartExplosionAsync;
        _draggableObject.PickedUp += StopExplosion;
    }
    
    public void EnableExplosion() => _canBeExploded = true;

    private void OnEnable() => _isFree = false;

    public void StartExplosionAsync() => StartExplosion().Forget();
    private async UniTask StartExplosion()
    {
        if (!_canBeExploded) return;
        
        _fuseVisualEffect.Play();
        
        try
        {
            await UniTask.WaitForSeconds(_explosion.GetOwnerEntity().StatContainer.Get<ExplotionDelay>().Value, cancellationToken: _cancellationTokenSource.Token);
            Explode();
        }
        catch (Exception e) { e.LogAsync(); } 
        
        
        _fuseVisualEffect.StopAsync().Forget();
    }
    
    private void StopExplosion()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }
    
    private async void Explode()
    {
        gameObject.SetActive(false);
        _canBeExploded = false;
        await _explosion.Explode();
        Exploded?.Invoke(this);
        
        _isFree = true;
    }
}