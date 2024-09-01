using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    private Transform _source;
    private Transform _target;

    public void SetTargets(Transform source, Transform target)
    {
        _target = target;

        _source = source;

        _lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, _source.position );

        _lineRenderer.SetPosition(1, _target.position+ new Vector3(0f, 0.5f, 0f));
    }
}
