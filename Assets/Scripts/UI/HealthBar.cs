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
        UpdateBar();
    }

    private void UpdateBar()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        if (_entityHealth.GetHp() < _displayedHealth)
        {
            Shake();
            DecreaseValue();
        }
        else if (_entityHealth.GetHp() > _displayedHealth)
        {
            IncreaseValue();
        }
    }

    private async void DecreaseValue()
    {
        _displayedHealth = _entityHealth.GetHp();

        await UniTask.WaitForSeconds(IdleTweenDuration, cancellationToken: _cancellationTokenSource.Token);

        DOTween.Kill(this);
        DOVirtual.Float(_healthDifference, _displayedHealth, HealthTweenDuration, UpdateHealthDifference);
    }

    private void UpdateHealthDifference(float value)
    {
        _healthDifference = value;
        UpdatePropertyBlock();
    }

    private async void IncreaseValue()
    {
        _healthDifference = _entityHealth.GetHp();

        await UniTask.WaitForSeconds(IdleTweenDuration, cancellationToken: _cancellationTokenSource.Token);

        DOTween.Kill(this);
        DOVirtual.Float(_displayedHealth, _healthDifference, HealthTweenDuration, UpdateDisplayedHealth);
    }

    private void UpdateDisplayedHealth(float value)
    {
        _healthDifference = value;
        UpdatePropertyBlock();
    }

    private void UpdatePropertyBlock()
    {
        _materialPropertyBlock.SetFloat("Health", _healthDifference);
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