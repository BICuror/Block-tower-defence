using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.VFX;
using UnityEngine;
using System;

public sealed class VisualEffectHandler : MonoBehaviour
{
    [SerializeField] private StopActionType _stopAction;
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private bool _detachFromParentWhilePlaying = false;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private Transform _initialParent;
    private Vector3 _initialLocalPosition;
    private Quaternion _initialLocalRotation;
    private bool _initialParentCaptured;
    private float _disableTime;

    private void Awake()
    {
        _disableTime = _visualEffect.GetFloat("MaxLifeTime");
        TryCaptureInitialParent();
    }

    #region BurstEffects

    public void PlayBurstEffectAndForget() => PlayBurstEffect().Forget();

    public async UniTask PlayBurstEffect()
    {
        CancelStopAction();
        PlayEffect();
        
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

    #endregion

    public void PlayEffect()
    {
        CancelStopAction();
        TryCaptureInitialParent();
        TryReturnToDefaultParent();
        TrySetNullParent();
        
        _visualEffect.gameObject.SetActive(true);
        _visualEffect.Stop();
        _visualEffect.Play();
    }
    
    #region PlayPermamentRegion
    
    public async UniTask StopPermamentEffect()
    {
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
    

    #endregion

    #region StopAction
    
    public void Stop()
    {
        _visualEffect.Stop();
        
        switch(_stopAction)
        {
            case StopActionType.DisableEffectObjectAndScriptEffect: DisableEffectObjectAndScriptObjects(); break;
            case StopActionType.DisableEffectObject: DisableEffectObject(); break;
            case StopActionType.None: break;
            case StopActionType.DestroyObject: Destroy(); return;
        }

        TryReturnToDefaultParent();
    }

    private void DisableEffectObject()
    {
        _visualEffect.gameObject.SetActive(false);
    }

    private void DisableEffectObjectAndScriptObjects()
    {
        _visualEffect.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    
    #endregion

    #region KeepIniitalParent

    private void TrySetNullParent()
    {
        if (!_detachFromParentWhilePlaying) return;    
        
        transform.SetParent(null);
    }

    private void TryCaptureInitialParent()
    {
        if (_initialParentCaptured || !_detachFromParentWhilePlaying) return;
            
        _initialParent = transform.parent;
        _initialLocalPosition = transform.localPosition;
        _initialLocalRotation = transform.localRotation;

        _initialParentCaptured = true;
    }
    
    private void TryReturnToDefaultParent()
    {
        if (!_detachFromParentWhilePlaying) return;
        
        transform.SetParent(_initialParent);
        transform.localPosition = _initialLocalPosition;
        transform.localRotation = _initialLocalRotation;
    }

    #endregion
    
    private void CancelStopAction()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    private void OnDestroy() => CancelStopAction();
    
    private enum StopActionType
    {
        DisableEffectObjectAndScriptEffect, 
        DestroyObject,
        DisableEffectObject,
        None
    }
}