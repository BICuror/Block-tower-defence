using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Combat;
using System;

public sealed class Bomb : DraggableObject
{  
    [SerializeField] private Explotion _explotion;
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _canBeExploded;

    public Action<Bomb> Exploded;
    
    public void Awake()
    {
        base.Awake();
        
        Placed += StartExplotion;
        PickedUp += StopExplotion;
    }
    
    public void EnableExplotion() => _canBeExploded = true;
    
    private async void StartExplotion()
    {
        if (!_canBeExploded) return;
        
        try
        {
            await UniTask.WaitForSeconds(_explotion.GetOwnerEntity().StatContainer.Get<ExplotionDelay>().Value, cancellationToken: _cancellationTokenSource.Token);
            Explode();
        }
        catch (Exception e) { e.LogAsync(); } 
    }
    
    private void StopExplotion()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }
    
    private async void Explode()
    {
        gameObject.SetActive(false);
        _canBeExploded = false;
        await _explotion.Explode();
        Exploded.Invoke(this);
    }
}