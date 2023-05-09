using UnityEngine;

public sealed class StaticUIElement : IngameUIElement
{
    private void Awake()
    {
        _cameraRotationController = FindObjectOfType<CameraRotationController>();

        _cameraRotationController.CameraChangedPosition.AddListener(LookAtCamera);
    }
    
    private void OnDestroy() => _cameraRotationController.CameraChangedPosition.RemoveListener(LookAtCamera);
}
