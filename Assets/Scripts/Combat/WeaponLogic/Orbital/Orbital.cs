using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using Combat;
using System;

public sealed class Orbital : WeaponBase
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Rigidbody _rigidbody;
    private CancellationTokenSource _cancellationTokenSource = new();
    private List<Transform> _targetTransforms;
    private int _currentTargetIndex;
    private ProjectileSpeed _speed;
    private Damage _damage;
    
    public void SetTravelPoints(List<Transform> targetTransforms)
    {
        _targetTransforms = targetTransforms;
    }

    public void SetNextTarget(Transform targetTransform)
    {
        _currentTargetIndex = _targetTransforms.IndexOf(targetTransform);
        TravelToNextTarget();
    }

    private async UniTask TravelToNextTarget()
    {
        CancelMovement();
        float duration = Vector3.Distance(transform.position, _targetTransforms[_currentTargetIndex].position) * _speed.Value;
        
        int previousTargetIndex = _currentTargetIndex - 1;
        if (previousTargetIndex < 0) previousTargetIndex = _targetTransforms.Count - 1;
        
        transform.DOMove(_targetTransforms[_currentTargetIndex].position, duration).SetEase(Ease.Linear).From(_targetTransforms[previousTargetIndex].position).AsyncWaitForCompletion();
        
        try
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: _cancellationTokenSource.Token);
            transform.DOComplete();
        }
        catch (Exception e)
        {
            TaskUtility.LogAsync(e);
            return;
        }
        
        _currentTargetIndex++;

        if (_currentTargetIndex >= _targetTransforms.Count) _currentTargetIndex = 0;

        TravelToNextTarget();
    }

    private void OnDisable()
    {
        _trailRenderer.Clear();
        CancelMovement();
    }
    private void OnEnable() => _trailRenderer.Clear();

    private void CancelMovement()
    {
        transform.DOComplete();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }
        
    protected override void OnInitialized()
    {
        _speed = OwnerEntity.StatContainer.Get<ProjectileSpeed>();
        _damage = OwnerEntity.StatContainer.Get<Damage>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyEntity enemyEntity))
        {
            DamageEntity(_damage.Value, enemyEntity);
        }
    }
}