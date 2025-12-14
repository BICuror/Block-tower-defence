using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using Cashing;
using Combat;
using System;

public abstract class HealthBar : Shaker
{
    [Cached] protected EntityHealth OwnerHealth;
    
    private const float HealthTweenDuration = 0.2f;
    private const float IdleTweenDuration = 0.3f;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private float _healthDifference = 1f;
    private float _displayedHealth = 1f;

    private bool _initialized;

    protected void Initialize()
    {
        base.Initialize();
        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);

        _initialized = true;
        
        UpdatePropertyBlock();
    }
    
    protected void UpdateBar()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
        
        DOTween.Kill(this);

        float hpPercent = OwnerHealth.GetHpPercent();

        if (hpPercent < _displayedHealth)
        {
            Shake();
            DecreaseValue();
        }
        else if (hpPercent > _displayedHealth)
        {
            IncreaseValue();
        }
    }

    private async void DecreaseValue()
    {
        _displayedHealth = OwnerHealth.GetHpPercent();
        UpdatePropertyBlock();

        try
        {
            await UniTask.WaitForSeconds(IdleTweenDuration, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }

        DOVirtual.Float(_healthDifference, _displayedHealth, HealthTweenDuration, UpdateHealthDifference);
    }
    
    private void IncreaseValue()
    {
        DOVirtual.Float(_displayedHealth, OwnerHealth.GetHpPercent(), HealthTweenDuration, value =>
        {
            _healthDifference = value;
            _displayedHealth = value;
            UpdatePropertyBlock();
        });
    }
    
    private void UpdateHealthDifference(float value)
    {
        _healthDifference = value;
        UpdatePropertyBlock();
    }
    
    private void UpdateDisplayedHealth(float value)
    {
        _displayedHealth = value;
        UpdatePropertyBlock();
    }

    private void UpdatePropertyBlock()
    {
        _materialPropertyBlock.SetFloat("Health", _displayedHealth);
        _materialPropertyBlock.SetFloat("HealthDifference", _healthDifference);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    protected void OnDisable()
    {
        base.OnDisable();
        
        _cancellationTokenSource.Cancel();
        
        if (!_initialized) return;
        
        UpdateDisplayedHealth(OwnerHealth.GetHpPercent());
        UpdateHealthDifference(OwnerHealth.GetHpPercent());
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        
        _cancellationTokenSource.Cancel();
    }
}