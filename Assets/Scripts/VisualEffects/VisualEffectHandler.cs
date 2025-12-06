using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.VFX;
using UnityEngine;

public sealed class VisualEffectHandler : MonoBehaviour
{
    [SerializeField] private StopActionType _stopAction;
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private bool _keepInitialParent;

    private CancellationTokenSource _cancellationTokenSource = new();
    private Transform _initialParent;
    private Vector3 _initialLocalPosition;
    private float _disableTime;

    private void Awake()
    {
        _disableTime = _visualEffect.GetFloat("MaxLifeTime"); 
    }
    
    public void PlayAndForget() => PlayAndStop().Forget();

    public async UniTask PlayAndStop()
    {
        Play();
        Cancel();
        
        try
        {
            await UniTask.WaitForSeconds(_disableTime, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }
        
        Stop();
    }
    
    public void Play()
    {
        if (!_keepInitialParent)
        {
            _initialParent = transform.parent;
            _initialLocalPosition = transform.localPosition;
        
            transform.SetParent(null);
        }
        
        _visualEffect.gameObject.SetActive(true);
        _visualEffect.Play();
    }

    public async UniTask StopAsync()
    {
        Cancel();
        
        _visualEffect.Stop();
        
        try
        {
            await UniTask.WaitForSeconds(_disableTime, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }
        
        Stop();
    }
    
    public void Stop()
    {
        switch(_stopAction)
        {
            case StopActionType.DisableSelfAndEffect: Disable(); break;
            case StopActionType.Destroy: Destroy(); break;
            case StopActionType.DisableEffect: Disable(); break;
            case StopActionType.None: break;
        } 
    }

    public void DisableEffect()
    {
        _visualEffect.gameObject.SetActive(false);

        TryReturnToDefaultParent();
    }

    private void Disable()
    {
        _visualEffect.gameObject.SetActive(false);

        gameObject.SetActive(false);

        TryReturnToDefaultParent();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    
    private void TryReturnToDefaultParent()
    {
        if (_keepInitialParent) return;
        
        transform.SetParent(_initialParent);
        transform.localPosition = _initialLocalPosition;
    }

    private void Cancel()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    private void OnDestroy() => Cancel();
    
    private enum StopActionType
    {
        DisableSelfAndEffect, 
        Destroy,
        DisableEffect,
        None
    }
}