using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using Cashing;
using Combat;
using System;

public class HealthBar : Shaker
{
    [Cached] protected EntityHealth OwnerHealth;
    
    private const float HealthTweenDuration = 0.2f;
    private const float IdleTweenDuration = 0.3f;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private float _healthDifference = 1f;
    private float _displayedHealth = 1f;

    protected void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);

        OwnerHealth.Damaged += UpdateBar;
        OwnerHealth.Healed += UpdateBar;
        OwnerHealth.Died += FillBar;
        
        UpdatePropertyBlock();
    }

    private void UpdateBar()
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
        catch (Exception e) { TaskUtility.LogAsync(e); }

        DOVirtual.Float(_healthDifference, _displayedHealth, HealthTweenDuration, UpdateHealthDifference);
    }

    private void UpdateHealthDifference(float value)
    {
        _healthDifference = value;
        UpdatePropertyBlock();
    }

    private async void IncreaseValue()
    {
        _healthDifference = OwnerHealth.GetHpPercent();
        UpdatePropertyBlock();

        try
        {
            await UniTask.WaitForSeconds(IdleTweenDuration, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e) { TaskUtility.LogAsync(e); }
        

        DOVirtual.Float(_displayedHealth, _healthDifference, HealthTweenDuration, UpdateDisplayedHealth);
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

    private void FillBar()
    {
        _displayedHealth = 1f;
        _healthDifference = 1f;
        UpdatePropertyBlock();
    }

    private void OnDisable() => _cancellationTokenSource.Cancel();

    private void OnDestroy()
    {
        OwnerHealth.Damaged -= UpdateBar;
        OwnerHealth.Healed -= UpdateBar;
        
        _cancellationTokenSource.Cancel();
    }
}