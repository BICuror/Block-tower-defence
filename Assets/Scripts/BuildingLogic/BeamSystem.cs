using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

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
 
        _lineRenderer.positionCount = 2;
        _targets[1] = target;

        if (_beamType == BeamType.Dynamic)
        {
            KeepUpBeamToTarget();
        }
        else
        {
            UpdateLinePositions();
        }
    }

    private async UniTask KeepUpBeamToTarget()
    {
        while (_targets[0] && _targets[1])
        {   
            UpdateLinePositions();

            try
            {
                await UniTask.WaitForFixedUpdate(cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                TaskUtility.LogAsync(e);
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

    private void UpdateLinePositions()
    {
        _lineRenderer.SetPosition(0, _targets[0].position);
        _lineRenderer.SetPosition(1, _targets[1].position);
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