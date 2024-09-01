using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public sealed class GPUInstancerEnabler : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private MaterialPropertyBlock _materialPropertyBlock; 

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable() => EnableGPUInstancing();

    public void EnableGPUInstancing()
    {
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
