using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private Camera _camera;
    [SerializeField] private CameraRotationController _cameraRotationController;

    private List<StaticUIElement> _staticUIElements;
    private List<DynamicUIElement> _dynamicUIElements;

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

    public void RotateElement(IngameUIElement element) => element.LookAt(_camera.transform.position); 

    private void Update()
    {
        for (int i = 0; i < _dynamicUIElements.Count; i++)
        {
            if (_dynamicUIElements[i].gameObject.activeSelf)
            {
                _dynamicUIElements[i].LookAt(_camera.transform.position);
            }
        }
    }

    private void OnCameraRotated()
    {
        for (int i = 0; i < _staticUIElements.Count; i++)
        {
            _staticUIElements[i].LookAt(_camera.transform.position);
        }
    }
}
