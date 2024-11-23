using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class WaveStateController : MonoBehaviour
{
    [Header("TransitionSettings")]
    [SerializeField] protected float TransitionOutDuration;
    [SerializeField] protected float TransitionInDuration;

    public Action EnteredStateStarted;
    public Action EnteredStateCompleted;

    public Action QuitStateStarted;
    public Action QuitStateCompleted;

    public virtual WaveState GetControlledState() => WaveState.None;
    
    public async UniTask TransitionIntoState()
    {
        OnEnterStateStarted();
        EnteredStateStarted?.Invoke();
        
        await UniTask.WaitForSeconds(TransitionInDuration, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        OnEnterStateCompleted();
        EnteredStateCompleted?.Invoke();
    }

    public async UniTask TransitionOutOfState()
    {
        OnQuitStateStarted();
        QuitStateStarted?.Invoke();
        
        await UniTask.WaitForSeconds(TransitionOutDuration, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        OnQuitStateCompleted();
        QuitStateCompleted?.Invoke();
    }
    
    protected virtual void OnEnterStateStarted() {}
    protected virtual void OnEnterStateCompleted() {}

    protected virtual void OnQuitStateStarted() {}
    protected virtual void OnQuitStateCompleted() {}
}