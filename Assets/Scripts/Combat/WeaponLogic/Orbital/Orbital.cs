using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using WorldGeneration;
using DG.Tweening;
using UnityEngine;
using Combat;
using System;

public sealed class Orbital : WeaponBase
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Rigidbody _rigidbody;
    
    [Header("TerrainFollowing")]
    [SerializeField] private AnimationCurve _upHeightChangeCurve;
    [SerializeField] private AnimationCurve _downHeightChangeCurve;
    private bool _followTerrain; 
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private IslandHeightMapHolder _islandHeightMapHolder;
    private List<Transform> _targetTransforms;
    private int _currentTargetIndex;
    private OrbitalSpeed _speed;
    private Damage _damage;
    
    public void SetTravelPoints(List<Transform> targetTransforms) => _targetTransforms = targetTransforms;
    
    public void SetIslandHeightMapHolder(IslandHeightMapHolder islandHeightMapHolder) => _islandHeightMapHolder = islandHeightMapHolder;
    
    public void SetFollowTerrainState(bool followTerrain) => _followTerrain = followTerrain;
    
    
    public void SetNextTarget(Transform targetTransform, Vector3 startPosition)
    {
        _currentTargetIndex = _targetTransforms.IndexOf(targetTransform);
        _trailRenderer.Clear();
     
        CancelMovement();
        TravelToNextTarget(new Vector2(startPosition.x, startPosition.z)).Forget();
        _trailRenderer.Clear();
    }

    private async UniTask TravelToNextTarget(Vector2 startPosition)
    {
        Vector2 endPosition = new Vector2(_targetTransforms[_currentTargetIndex].position.x, _targetTransforms[_currentTargetIndex].position.z);

        float distance = Vector2.Distance(startPosition, endPosition);
        float duration = distance * _speed.Value;
        float stepDuration = duration / distance;
        float elapsedTime = 0;
        int currentStep = 0;

        float startTileY = 0f;
        float endTileY = 0f;
        float currentStepDuration = 0f;
        
        while (duration > elapsedTime)
        {
            try
            {
                await UniTask.WaitForFixedUpdate(cancellationToken: _cancellationTokenSource.Token);
                transform.DOComplete();
            }
            catch (Exception e)
            {
                e.LogAsync();
                return;
            }
            
            if (_followTerrain)
            {
                if (stepDuration * currentStep <= elapsedTime)
                {
                    currentStep++;
    
                    int startX = Mathf.RoundToInt(transform.position.x);
                    int startZ = Mathf.RoundToInt(transform.position.z);
    
                    int endX = Mathf.RoundToInt(Vector2.Lerp(startPosition, endPosition, currentStep / distance).x);
                    int endZ = Mathf.RoundToInt(Vector2.Lerp(startPosition, endPosition, currentStep / distance).y);

                    startTileY = _islandHeightMapHolder.GetHeightSafe(startX, startZ);
                    endTileY = _islandHeightMapHolder.GetHeightSafe(endX, endZ);

                    if (startTileY == 0) startTileY = 1;
                    if (endTileY == 0) endTileY = 1;
                    
                    currentStepDuration = 0f;
                }
            }
            else
            {
                startTileY = transform.parent.position.y - 1;
                endTileY = transform.parent.position.y - 1;
            }

            float height = GetOrbitalHeight(startTileY, endTileY, currentStepDuration / stepDuration);
            
            MoveToNextTarget(startPosition, endPosition, height, elapsedTime / duration);
            
            elapsedTime += Time.fixedDeltaTime;
            currentStepDuration += Time.fixedDeltaTime;
        }
        
        MoveToNextTarget(startPosition, endPosition, GetOrbitalHeight(startTileY, endTileY, 1f), 1f);

        IncreaseTargetIndex();

        TravelToNextTarget(endPosition).Forget();
    }

    private float GetOrbitalHeight(float startTileY, float endTileY, float progress)
    {
        if (startTileY == endTileY)
        {
            return endTileY;
        }
        if (startTileY > endTileY)
        {
            return Mathf.Lerp(startTileY, endTileY, _downHeightChangeCurve.Evaluate(progress));
        } 
        
        return Mathf.Lerp(startTileY, endTileY,_upHeightChangeCurve.Evaluate(progress));
    }

    private void MoveToNextTarget(Vector2 startPosition, Vector2 endPosition, float height, float progress)
    {
        Vector2 position = Vector2.Lerp(startPosition, endPosition, progress);
            
        transform.position = new Vector3(position.x, height + 1f, position.y);
    }

    private void IncreaseTargetIndex()
    {
        _currentTargetIndex++;

        if (_currentTargetIndex >= _targetTransforms.Count) _currentTargetIndex = 0;
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