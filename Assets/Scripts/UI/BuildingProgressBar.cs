using DG.Tweening;
using UnityEngine;
using Cashing;

[RequireComponent(typeof(MeshRenderer))]

public sealed class BuildingProgressBar : Shaker
{
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private BuildTime _buildTime;
    [SerializeField] private Material _progressBarMaterial;
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    private Tween _currentTween;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.sharedMaterial = _progressBarMaterial;
        _materialPropertyBlock = new MaterialPropertyBlock();
        
        _buildingDraggable.Placed += StartFillingBar;
        _buildingDraggable.PickedUp += StopFillingBar;
        
        gameObject.SetActive(false);
    }

    private void StartFillingBar()
    {
        gameObject.SetActive(true);

        Shake();

        _currentTween = DOVirtual.Float(1f, 0f, _buildTime.Value, SetPropertyBlock).SetEase(Ease.Linear).OnComplete(StopFillingBar);
    }

    private void StopFillingBar()
    {
        if (_currentTween != null && _currentTween.IsPlaying()) _currentTween.Complete();

        transform.DOComplete();

        gameObject.SetActive(false);
    }

    private void SetPropertyBlock(float progressValue)
    {
        _materialPropertyBlock.SetFloat("BuildProgress", progressValue);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void OnDestroy()
    {
        _buildingDraggable.Placed -= StartFillingBar;
        _buildingDraggable.PickedUp -= StopFillingBar;
        StopFillingBar();
    }
}