using System.Collections.Generic;
using UnityEngine;

public sealed class DecorationObject : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    
    public void SetMaterial(Material material)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        _meshRenderers.ForEach(renderer =>
        {
            renderer.sharedMaterial = material;
            renderer.SetPropertyBlock(block);
        });
    }
}