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
    private OrbitalSpeed _speed;
    private Damage _damage;
    
    public void SetTravelPoints(List<Transform> targetTransforms)
    {
        _targetTransforms = targetTransforms;
    }

    public void SetNextTarget(Transform targetTransform)
    {
        _currentTargetIndex = _targetTransforms.IndexOf(targetTransform);
        _trailRenderer.Clear();
        TravelToNextTarget(transform.position);
    }

    private async UniTask TravelToNextTarget(Vector3 startPosition)
    {
        CancelMovement();
        float duration = Vector3.Distance(transform.position, _targetTransforms[_currentTargetIndex].position) * _speed.Value;
        
        transform.DOMove(_targetTransforms[_currentTargetIndex].position, duration).SetEase(Ease.Linear).From(startPosition);
        
        try
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: _cancellationTokenSource.Token);
            transform.DOComplete();
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }
        
        Vector3 endPosition = _targetTransforms[_currentTargetIndex].position;
        
        _currentTargetIndex++;

        if (_currentTargetIndex >= _targetTransforms.Count) _currentTargetIndex = 0;

        TravelToNextTarget(endPosition);
    }

    private void OnDisable()
    {
        _trailRenderer.Clear();
        CancelMovement();
    }

    private void OnEnable()
    {
        _trailRenderer.Clear();
    }

    public void CancelMovement()
    {
        transform.DOKill();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }
        
    protected override void OnInitialized()
    {
        _speed = OwnerEntity.StatContainer.Get<OrbitalSpeed>();
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