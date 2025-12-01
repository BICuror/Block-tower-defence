using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System.Threading;
using UnityEngine;
using System;

public sealed class Launcher : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1f;
    [SerializeField] private float _maxHeight = 4f;
    
    private CancellationTokenSource _cancellationTokenSource = new();
     
    public UnityEvent Landed;
    
    public async UniTask Launch(Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _lifeTime)
        {
            elapsedTime += Time.deltaTime;

            float currentProgress = elapsedTime / _lifeTime;

            Vector3 evaluetedPosition = Vector3.Lerp(startPosition, endPosition, currentProgress);

            evaluetedPosition.y += (Mathf.Sin(currentProgress * 180f * Mathf.Deg2Rad)) * _maxHeight; 

            transform.position = evaluetedPosition;

            await UniTask.WaitForFixedUpdate(_cancellationTokenSource.Token).SuppressCancellationThrow();
        }

        Landed.Invoke();
    }

    private void OnDestroy() => _cancellationTokenSource.Cancel();
}