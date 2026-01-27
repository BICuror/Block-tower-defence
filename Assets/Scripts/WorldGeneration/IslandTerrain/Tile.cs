using NaughtyAttributes;
using UnityEngine;

public sealed class Tile : MonoBehaviour
{
    [SerializeField] private float _scaleMultiplier = 0.5f;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _mainMaterial;
    [SerializeField] private bool _hasTransition;
    [ShowIf("_hasTransition")] [SerializeField] private Material _transitionMaterial;

    public void SetScale(float xScale, float zScale)
    {
        transform.localScale = new Vector3(xScale * _scaleMultiplier, 1f, zScale * _scaleMultiplier);
    }
    
    public void SetTransitionMaterial() => SetMaterial(_transitionMaterial);
    public void SetMainMaterial() => SetMaterial(_mainMaterial);
    
    private void SetMaterial(Material material)
    {
        _meshRenderer.sharedMaterial = material;
        _meshRenderer.SetPropertyBlock(new MaterialPropertyBlock());
    }
}