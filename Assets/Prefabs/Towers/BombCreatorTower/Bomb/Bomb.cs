using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Combat;
using System;

public sealed class Bomb : WeaponBase
{  
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
        
        _draggableObject.Placed += StartExplotion;
        _draggableObject.PickedUp += StopExplosion;
    }
    
    public void EnableExplotion() => _canBeExploded = true;

    private void OnEnable()
    {
        _isFree = false;
    }
    
    private async void StartExplotion()
    {
        if (!_canBeExploded) return;
        
        try
        {
            await UniTask.WaitForSeconds(_explosion.GetOwnerEntity().StatContainer.Get<ExplotionDelay>().Value, cancellationToken: _cancellationTokenSource.Token);
            Explode();
        }
        catch (Exception e) { e.LogAsync(); } 
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
        Exploded.Invoke(this);
        
        _isFree = true;
    }
}