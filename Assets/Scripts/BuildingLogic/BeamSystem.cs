using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
using JetBrains.Annotations;

public sealed class BeamSystem : MonoBehaviour
{
    [SerializeField] private BeamType _beamType;
    [SerializeField] private Material _beamMaterial;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private MeshRenderer[] _additionalRenderers;
    private CancellationTokenSource _cancellationTokenSource = new();
    private Transform[] _targets = new Transform[2];

    private void Awake()
    {
        _beamMaterial = new Material(_beamMaterial);

        _lineRenderer.sharedMaterial = _beamMaterial;

        foreach (MeshRenderer renderer in _additionalRenderers)
        {
            renderer.sharedMaterial = _beamMaterial;
        }
    }

    public void SetAlpha(float alpha)
    {
        _beamMaterial.SetFloat("Alpha", alpha);
    }

    public void SetSource(Transform source)
    {
        _targets[0] = source;    
    }
    
    public void SetTarget(Transform target)
    {
        DisableBeam();
 
        _targets[1] = target;

        if (_beamType == BeamType.Dynamic)
        {
            KeepUpBeamToTarget().Forget();
        }
        else
        {
            UpdateLinePositions(_targets[0].position, _targets[1].position);
        }
    }

    private async UniTask KeepUpBeamToTarget()
    {
        while (_targets[0] && _targets[1])
        {   
            UpdateLinePositions(_targets[0].position, _targets[1].position);

            try
            {
                await UniTask.WaitForFixedUpdate(cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                break;
            }
        }
    }

    public void DisableBeam()
    {
        _lineRenderer.positionCount = 0;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }

    private void UpdateLinePositions(Vector3 startPosition, Vector3 endPosition)
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }

    public async UniTask ReachTargetAndSetIt( Vector3 startPosition, [CanBeNull] Transform target, float reachDuration)
    {
        DisableBeam();
        
        float elapsedTime = 0f; 
        float halfDuration = reachDuration / 2f;
        
        while (reachDuration > elapsedTime)
        {
            try
            {
                await UniTask.WaitForFixedUpdate(_cancellationTokenSource.Token);
            }
            catch { return; }
            
            elapsedTime += Time.fixedDeltaTime;
            
            if (!target || !target.gameObject.activeSelf) return;

            float startTime = Mathf.Clamp(elapsedTime / halfDuration, 0f, 1f);
            float endTime = Mathf.Clamp(elapsedTime / halfDuration - 1f, 0f, 1f);

            Vector3 currentStartPosition = Vector3.Lerp(startPosition, target.position, startTime);
            Vector3 currentEndPosition = Vector3.Lerp(startPosition, target.position, endTime);
            
            UpdateLinePositions(currentEndPosition, currentStartPosition);
        }
        
        SetTarget(target);
    }
    
    public async UniTask ReachPosition(Vector3 position, Vector3 startPosition, float reachDuration)
    {
        DisableBeam();   
        
        float elapsedTime = 0f; 
        
        while (reachDuration > elapsedTime)
        {
            try
            {
                await UniTask.WaitForFixedUpdate(_cancellationTokenSource.Token);
            }
            catch { return; }
            
            elapsedTime += Time.fixedDeltaTime;

            Vector3 currentPosition = Vector3.Lerp(position, startPosition, elapsedTime / reachDuration);
            
            UpdateLinePositions(_targets[0].position, currentPosition);
        }
    }

    private void OnDestroy()
    {
        DisableBeam();
    }
}

public enum BeamType
{
    Dynamic,
    Static
} 