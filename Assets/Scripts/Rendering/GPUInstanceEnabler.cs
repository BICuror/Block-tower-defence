using UnityEngine;

[RequireComponent(typeof(Renderer))]

public sealed class GPUInstanceEnabler : MonoBehaviour
{
    private Renderer _renderer;

    private MaterialPropertyBlock _materialPropertyBlock; 

    public Renderer Renderer => _renderer;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable() => EnableGPUInstancing();

    public void EnableGPUInstancing()
    {
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
