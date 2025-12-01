using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(ItemsContainer))]

public sealed class ItemsContainerAnimator : MonoBehaviour
{
    [SerializeField] private ItemsContainer _itemsContainer;
    [SerializeField] private AnimationCurve _movmentCurve;
    [SerializeField] private AnimationCurve _radiusCurve;
    [SerializeField] private float _transitionDuration = 1.6f;
    [SerializeField] private int _maxItems;
    [SerializeField] private float _maxRaduis = 1.4f;
    [SerializeField] private float _defultRotationSpeed = 0.2f;
    private float _currentRotationSpeed;
    
    private float GetRadius() => _radiusCurve.Evaluate(_itemsContainer.ContainedItems.Count / (float)_maxItems) * _maxRaduis;

    public void TransitionToNewPositions()
    {
        StopAllCoroutines();

        StartCoroutine(StartTransitioningToNewPositions());
    }

    private List<Vector3> GetNewPositions()
    {
        List<Vector3> newPositions = new();

        float angle = 0f;
        float step = 360f / _itemsContainer.ContainedItems.Count;

        for (int i = 0; i < _itemsContainer.ContainedItems.Count; i++)
        {
            var radians = Math.PI * (step * i) / 180.0;
            var cos = (float)Math.Round(Math.Cos(radians), 2);
            var sin = (float)Math.Round(Math.Sin(radians), 2);
        
            float radius = GetRadius();
            
            newPositions.Add(new Vector3(cos * radius, 0f, sin * radius));
        }

        return newPositions;
    }

    private List<Vector3> GetOldPositions()
    {
        List<Vector3> oldPositions = new();

        for (int i = 0; i < _itemsContainer.ContainedItems.Count; i++)
        {
            oldPositions.Add(_itemsContainer.ContainedItems[i].transform.localPosition);
        }

        return oldPositions;
    }

    private IEnumerator StartTransitioningToNewPositions()
    {
        if (_itemsContainer.ContainedItems.Count == 0) StopAllCoroutines();
        
        List<Vector3> oldPositions = GetOldPositions();
        List<Vector3> newPositions = GetNewPositions();

        float elapsedTime = 0f;
        float evaluatedTime = 0f;

        while (elapsedTime < _transitionDuration)
        {
            evaluatedTime = elapsedTime / _transitionDuration;

            Transition(evaluatedTime);

            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        Transition(1f);

        void Transition(float evaluatedTime)
        {
            for (int i = 0; i < _itemsContainer.ContainedItems.Count; i++)
            {
                _itemsContainer.ContainedItems[i].transform.localPosition = Vector3.Lerp(oldPositions[i], newPositions[i], _movmentCurve.Evaluate(evaluatedTime));
            }
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, _currentRotationSpeed, 0f));
        
        _itemsContainer.ContainedItems.ForEach(item => item.transform.rotation = Quaternion.identity);
    }

    public void StopRotation() => _currentRotationSpeed = 0f;
    
    public void StartRotation() => _currentRotationSpeed = _defultRotationSpeed;
}