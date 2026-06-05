using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Renderer))]

public abstract class ProgressBarBase : Shaker
{
    [SerializeField] private Material _barMaterial;
    [SerializeField] private Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    protected Tween CurrentTween;
    
    protected abstract string ProgressFieldName { get; }
    
    protected void Initialize()
    {
        base.Initialize();
        
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        _renderer.sharedMaterial = _barMaterial;
        _materialPropertyBlock = new MaterialPropertyBlock();
    }
    
    protected async UniTask FillBar(float initialValue, float finalValue, float tweenDuration)
    {
        StopBarFill();
        
        CurrentTween = DOVirtual.Float(initialValue, finalValue, tweenDuration, SetPropertyBlock).SetEase(Ease.Linear).OnComplete(StopBarFill);

        await CurrentTween.AsyncWaitForCompletion();
        
        OnFillComplete();
    }
    
    protected void StopBarFill()
    {
        if (CurrentTween != null && CurrentTween.IsPlaying()) CurrentTween.Complete();
    }

    protected virtual void OnFillComplete() {}

    private void SetPropertyBlock(float progressValue)
    {
        _materialPropertyBlock.SetFloat(ProgressFieldName, progressValue);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        
        StopBarFill();
    }
}