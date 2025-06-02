using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public abstract class ProgressBarBase : Shaker
{
    [SerializeField] private Material _barMaterial;
    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;
    protected Tween CurrentTween;
    
    protected abstract string ProgressFieldName { get; }
    
    protected void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.sharedMaterial = _barMaterial;
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

    protected abstract void OnFillComplete();

    private void SetPropertyBlock(float progressValue)
    {
        _materialPropertyBlock.SetFloat(ProgressFieldName, progressValue);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}