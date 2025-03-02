using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(ItemsContainer))]

public class ItemsContainerAnimator : MonoBehaviour
{
    [SerializeField] private AnimationCurve _movmentCurve;
    [SerializeField] private AnimationCurve _radiusCurve;
    [SerializeField] private int _maxItems;
    [SerializeField] private float _maxRaduis = 1.4f;
    
    public float GetRadius(int itemsCount) => _radiusCurve.Evaluate(itemsCount / (float)_maxItems) * _maxRaduis;

    public void TransitionToNewPositions(List<Item> items, float duration)
    {
        StopAllCoroutines();

        StartCoroutine(StartTransitioningToNewPositions(items, duration));
    }

    private List<Vector3> GetNewPositions(List<Item> items)
    {
        List<Vector3> newPositions = new();

        float angle = 0f;
        float step = 360f / items.Count;

        for (int i = 0; i < items.Count; i++)
        {
            var radians = Math.PI * (step * i) / 180.0;
            var cos = (float)Math.Round(Math.Cos(radians), 2);
            var sin = (float)Math.Round(Math.Sin(radians), 2);
        
            float radius = GetRadius(items.Count);
            
            newPositions.Add(new Vector3(cos * radius, 0f, sin * radius));
        }

        return newPositions;
    }

    private List<Vector3> GetOldPositions(List<Item> items)
    {
        List<Vector3> oldPositions = new();

        for (int i = 0; i < items.Count; i++)
        {
            oldPositions.Add(items[i].transform.localPosition);
        }

        return oldPositions;
    }

    private IEnumerator StartTransitioningToNewPositions(List<Item> items, float duration)
    {
        if (items.Count == 0) StopAllCoroutines();
        
        List<Vector3> oldPositions = GetOldPositions(items);
        List<Vector3> newPositions = GetNewPositions(items);

        float elapsedTime = 0f;
        float evaluatedTime = 0f;

        while (elapsedTime < duration)
        {
            evaluatedTime = elapsedTime / duration;

            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.localPosition = Vector3.Lerp(oldPositions[i], newPositions[i], _movmentCurve.Evaluate(evaluatedTime));
            }

            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
