using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class WaveStateController : MonoBehaviour
{
    [Header("TransitionSettings")]
    [SerializeField] protected float TransitionOutDuration;
    [SerializeField] protected float TransitionInDuration;

    public event Action EnteredStateStarted;
    public event Action EnteredStateCompleted;

    public event Action QuitStateStarted;
    public event Action QuitStateCompleted;

    public virtual WaveState GetControlledState() => WaveState.None;
    
    public async UniTask TransitionIntoState()
    {
        await OnEnterStateStarted();
        EnteredStateStarted?.Invoke();
        
        await UniTask.WaitForSeconds(TransitionInDuration, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        await OnEnterStateCompleted();
        EnteredStateCompleted?.Invoke();
    }

    public async UniTask TransitionOutOfState()
    {
        QuitStateStarted?.Invoke();
        await OnQuitStateStarted();
        
        await UniTask.WaitForSeconds(TransitionOutDuration, cancellationToken: this.GetCancellationTokenOnDestroy());
        
        await OnQuitStateCompleted();
        QuitStateCompleted?.Invoke();
    }
    
    protected virtual async UniTask OnEnterStateStarted() {}
    protected virtual async UniTask OnEnterStateCompleted() {}

    protected virtual async UniTask OnQuitStateStarted() {}
    protected virtual async UniTask OnQuitStateCompleted() {}
}