using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class IngameUIElementManager : MonoBehaviour
{
    #region Singletone
    private static IngameUIElementManager _instance;
    public static IngameUIElementManager Instance => _instance;
    private void CreateSingletoneInstance()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else 
        {
            Debug.LogError("Multiple instances of IngameUIElementManager");
        }
    }
    #endregion
    
    [Inject] private CameraRotationController _cameraRotationController;

    [SerializeField] private Camera _camera;

    private List<StaticUIElement> _staticUIElements;
    private List<DynamicUIElement> _dynamicUIElements;

    public Action StaticElementsUpdated;

    private void Awake()
    {
        _staticUIElements = new List<StaticUIElement>();
        _dynamicUIElements = new List<DynamicUIElement>();

        CreateSingletoneInstance();

        _cameraRotationController.CameraRotated.AddListener(OnCameraRotated);
    }

    public void AddStaticUIElement(StaticUIElement element)
    {
        _staticUIElements.Add(element);
        RotateElement(element);
    }
    public void RemoveStaticUIElement(StaticUIElement element) => _staticUIElements.Remove(element);

    public void AddDynamicUIElement(DynamicUIElement element)
    {
        _dynamicUIElements.Add(element);
        RotateElement(element);
    }
    public void RemoveDynamicUIElement(DynamicUIElement element) => _dynamicUIElements.Remove(element);

    public void RotateElement(IngameUIElement element)
    {
        // Quirky way
        // element.LookAt(_camera.transform.position);
        
        // Normal way
        Vector2 cameraPosition = _camera.WorldToScreenPoint(element.transform.position);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(cameraPosition);
        element.LookAt(worldPosition);
    }

    private void Update()
    {
        for (int i = 0; i < _dynamicUIElements.Count; i++)
        {
            if (_dynamicUIElements[i].gameObject.activeSelf)
            {
                RotateElement(_dynamicUIElements[i]);
            }
        }
    }

    private void OnCameraRotated()
    {
        for (int i = 0; i < _staticUIElements.Count; i++)
        {
            RotateElement(_staticUIElements[i]);
        }
        
        StaticElementsUpdated?.Invoke();
    }
}
