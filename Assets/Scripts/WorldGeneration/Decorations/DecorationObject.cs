using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public sealed class DecorationObject : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    
    [Header("StateCheck")]
    [SerializeField] private bool _requiresStateCheck;
    [ShowIf("_requiresStateCheck")] [SerializeField] private float _tileScale = 1f;
    [ShowIf("_requiresStateCheck")] [SerializeField] private LayerSetting _decorationLayerSetting;
    private Vector3 _defaultScale;
    
    private void Start() => _defaultScale = transform.localScale;
    
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
        Debug.Log($"Setting state {state}");
        
        if (state) state = GetPlaceAvailability(transform.position);
        
        if (state) Appear();
        else Disappear();
    }
    
    public bool GetPlaceAvailability(Vector3 position)
    {
        if (!_requiresStateCheck) return true;
        
        float halfExtent = _tileScale * 0.5f;
            
        RaycastHit[] raycastHits = Physics.BoxCastAll(position, new Vector3(halfExtent, 100f, halfExtent), Vector3.zero, Quaternion.identity, 100f, _decorationLayerSetting.GetLayerMask());
        
        return raycastHits.Length == 0 || (raycastHits.Length == 1 && raycastHits[0].transform == transform);
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