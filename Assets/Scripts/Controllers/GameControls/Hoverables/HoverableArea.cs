using System;
using UnityEngine;

public sealed class HoverableArea : HoverableObject
{
    [Header("EnabledMaterial")]
    [SerializeField] private Material _enabledAreaMaterial;
    [SerializeField] private Material _enabledAreaFillMaterial;
    
    [Header("DisabledMaterial")]
    [SerializeField] private Material _disabledAreaMaterial;
    [SerializeField] private Material _disbledAreaFillMaterial;
    
    [Header("MeshRenderers")]
    [SerializeField] private MeshRenderer _areaMeshRenderer;
    [SerializeField] private MeshRenderer _areaFillMeshRenderer;
    
    [Header("GPUInstancing")]
    [SerializeField] private GPUInstanceEnabler _areaMeshGpuInstanceEnabler;
    [SerializeField] private GPUInstanceEnabler _areaFillMeshGpuInstanceEnabler;

    private void Start()
    {
        SetAreaMaterials(_disabledAreaMaterial, _disbledAreaFillMaterial);
        
        HoverEntered.AddListener(OnHoverEnter);
        HoverExited.AddListener(OnHoverExit);
    }
    
    private void OnHoverEnter() => SetAreaMaterials(_enabledAreaMaterial, _enabledAreaFillMaterial);

    private void OnHoverExit() => SetAreaMaterials(_disabledAreaMaterial, _disbledAreaFillMaterial);

    private void SetAreaMaterials(Material areaMaterial, Material areaFillMaterial)
    {
        _areaMeshRenderer.sharedMaterial = areaMaterial;
        _areaMeshGpuInstanceEnabler.EnableGPUInstancing();
        
        _areaFillMeshRenderer.sharedMaterial = areaFillMaterial;
        _areaFillMeshGpuInstanceEnabler.EnableGPUInstancing();
    }

    private void OnDestroy()
    {
        HoverEntered.RemoveListener(OnHoverEnter);
        HoverExited.RemoveListener(OnHoverExit);
    }
}