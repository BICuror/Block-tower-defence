using UnityEngine;

public sealed class Tile : MonoBehaviour
{
    [SerializeField] private float _scaleMultiplier = 0.5f;
    [SerializeField] private MeshRenderer _meshRenderer;
    
    public void SetScale(float xScale, float zScale)
    {
        transform.localScale = new Vector3(xScale * _scaleMultiplier, 1f, zScale * _scaleMultiplier);
    }
    
    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
        _meshRenderer.SetPropertyBlock(new MaterialPropertyBlock());
    }
}