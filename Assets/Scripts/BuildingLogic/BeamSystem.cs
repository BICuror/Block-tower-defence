using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BeamSystem : MonoBehaviour
{
    [SerializeField] private BeamType _beamType;

    [SerializeField] private Material _beamMaterial;

    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private Transform _beamSource;

    [SerializeField] private MeshRenderer[] _additionalRenderers;

    private Transform _target;

    private YieldInstruction _rechargeInstruction;

    private void Awake()
    {
        _rechargeInstruction = new WaitForFixedUpdate();

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

    public void SetTarget(Transform target)
    {
        StopAllCoroutines();
 
        _lineRenderer.positionCount = 2;

        _target = target;

        UpdateLinePositions();

        if (_beamType == BeamType.Dynamic) StartCoroutine(KeepUpBeamToTarget());
    }

    public void DisableBeam()
    {
        StopAllCoroutines();

        _lineRenderer.positionCount = 0;

        _target = null;

        SetAlpha(0);
    }

    private IEnumerator KeepUpBeamToTarget()
    {
        while (true)
        {   
            UpdateLinePositions();

            yield return _rechargeInstruction;
        }
    }

    private void UpdateLinePositions()
    {
        _lineRenderer.SetPosition(0, _beamSource.position);

        _lineRenderer.SetPosition(1, _target.position);
    }
}

public enum BeamType
{
    Dynamic,
    Static
} 
