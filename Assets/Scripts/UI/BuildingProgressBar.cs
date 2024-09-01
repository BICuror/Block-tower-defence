using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public sealed class BuildingProgressBar : Shaker
{
    [SerializeField] private Material _progressBarMaterial;
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    private Tween _currentTween;

    private void Awake()
    { 
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.sharedMaterial = _progressBarMaterial;
        _materialPropertyBlock = new MaterialPropertyBlock();

        CaptureDefaultScale();
    }

    private void SetPropertyBlock(float progressValue)
    {
        _materialPropertyBlock.SetFloat("BuildProgress", progressValue);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void StartFillingBar(float duration)
    {
        gameObject.SetActive(true);

        ShakeDuration = duration;
        Shake();
        
        _currentTween = DOVirtual.Float(1f, 0f, duration, SetPropertyBlock).SetEase(Ease.Linear).OnComplete(StopFillingBar);
    }

    public void StopFillingBar()
    {
        if (_currentTween != null && _currentTween.IsPlaying()) _currentTween.Kill();

        transform.DOKill();

        gameObject.SetActive(false);
    }

    private void OnDestroy() => StopFillingBar();
}
