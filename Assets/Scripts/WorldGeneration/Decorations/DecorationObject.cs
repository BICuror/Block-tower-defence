using System.Collections.Generic;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine;

public sealed class DecorationObject : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    
    [Header("StateCheck")]
    [SerializeField] private int _tileRadius;
    private Vector3 _defaultScale;
    
    public int TileRadius => _tileRadius;
    
    public void SetDefaultScale(Vector3 defaultScale) => _defaultScale = defaultScale;
    
    public void SetMaterial(Material material)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        _meshRenderers.ForEach(renderer =>
        {
            renderer.sharedMaterial = material;
            renderer.SetPropertyBlock(block);
        });
    }

    public void SetState(bool state)
    {
        if (state) Appear();
        else Disappear();
    }

    private void Appear()
    {
        if (gameObject.activeSelf) return;
        
        transform.DOKill();
        
        gameObject.SetActive(true);
        transform.DOScale(_defaultScale, 0.5f).From(Vector3.zero).SetLink(gameObject);
    }

    private void Disappear()
    {
        if (!gameObject.activeSelf) return;
        
        transform.DOKill();
        
        gameObject.SetActive(true);
        transform.DOScale(Vector3.zero, 0.5f).From(_defaultScale).SetLink(gameObject).OnComplete(() => gameObject.SetActive(false));
    }
}