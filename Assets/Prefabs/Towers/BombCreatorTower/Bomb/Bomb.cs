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
    
    [Header("Visualisation")] 
    [SerializeField] private AreaVisualisation _areaVisualisation;
    [SerializeField] private HoverableObject _hoverableObject;
    
    public bool IsFree => _isFree;
    public DraggableObject DraggableObject => _draggableObject;
    
    public event Action<Bomb> ExplosionStarted;
    public event Action<Bomb> ExplosionFinished;
    
    protected override void OnInitialized()
    {
        _explosion.Initialize(OwnerEntity);

        OwnerEntity.StatContainer.Get<ExplosionRadius>().ValueChanged += UpdateVisualisationScale;
        UpdateVisualisationScale(_explosion.ExplosionRadius);
        
        _areaVisualisation.SubscribeToHoverable(_hoverableObject);
        
        _draggableObject.Placed += StartExplosionAsync;
        _draggableObject.PickedUp += StopExplosion;
    }

    private void UpdateVisualisationScale(float _) => _areaVisualisation.SetDefaultScale(_explosion.ExplosionRadius * 2f);
    
    public void EnableExplosion() => _canBeExploded = true;

    private void OnEnable() => _isFree = false;

    public void StartExplosionAsync() => StartExplosion().Forget();
    private async UniTask StartExplosion()
    {
        if (!_canBeExploded) return;
        
        _fuseVisualEffect.PlayEffect();
        
        try
        {
            await UniTask.WaitForSeconds(_explosion.GetOwnerEntity().StatContainer.Get<ExplosionDelay>().Value, cancellationToken: _cancellationTokenSource.Token);
            Explode();
        }
        catch (Exception e) { e.LogAsync(); } 
        
        
        _fuseVisualEffect.StopPermamentEffect().Forget();
    }
    
    private void StopExplosion()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }
    
    private async void Explode()
    {
        ExplosionStarted?.Invoke(this);
        
        gameObject.SetActive(false);
        _canBeExploded = false;
        await _explosion.Explode();
        
        ExplosionFinished?.Invoke(this);
        
        _isFree = true;
    }

    private void OnDestroy()
    {
        OwnerEntity.StatContainer.Get<ExplosionRadius>().ValueChanged -= UpdateVisualisationScale;
        
        _draggableObject.Placed -= StartExplosionAsync;
        _draggableObject.PickedUp -= StopExplosion;
    }
}