using System.Threading;
using Cashing;
using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public sealed class HealthBar : Shaker
{
    private const float HealthTweenDuration = 0.2f;
    private const float IdleTweenDuration = 0.3f;

    [Cached] private EntityHealth _entityHealth;

    private CancellationTokenSource _cancellationTokenSource = new();
    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private float _healthDifference = 1f;
    private float _displayedHealth = 1f;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);

        _entityHealth.Damaged += UpdateBar;
        _entityHealth.Healed += UpdateBar;
        UpdatePropertyBlock();
    }

    private void UpdateBar()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
        
        DOTween.Kill(this);

        float hpPercent = _entityHealth.GetHpPercent();

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
        _displayedHealth = _entityHealth.GetHpPercent();
        UpdatePropertyBlock();

        await UniTask.WaitForSeconds(IdleTweenDuration, cancellationToken: _cancellationTokenSource.Token);

        DOVirtual.Float(_healthDifference, _displayedHealth, HealthTweenDuration, UpdateHealthDifference);
    }

    private void UpdateHealthDifference(float value)
    {
        _healthDifference = value;
        UpdatePropertyBlock();
    }

    private async void IncreaseValue()
    {
        _healthDifference = _entityHealth.GetHpPercent();
        UpdatePropertyBlock();

        await UniTask.WaitForSeconds(IdleTweenDuration, cancellationToken: _cancellationTokenSource.Token);

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

    private void OnDisable() => _cancellationTokenSource.Cancel();

    private void OnDestroy()
    {
        _entityHealth.Damaged -= UpdateBar;
        _entityHealth.Healed -= UpdateBar;
        
        _cancellationTokenSource.Cancel();
    }
}