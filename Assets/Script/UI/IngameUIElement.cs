using UnityEngine;

public class IngameUIElement : MonoBehaviour
{
    protected CameraRotationController _cameraRotationController;

    private void OnEnable()
    {
        _cameraRotationController = FindObjectOfType<CameraRotationController>();

        LookAtCamera();
    }

    protected void LookAtCamera()
    {
        Vector3 forward = transform.position - _cameraRotationController.transform.position;
        forward.Normalize();
        Vector3 up = Vector3.Cross(forward, _cameraRotationController.transform.right);
        transform.rotation = Quaternion.LookRotation(forward, up);
        
        transform.Rotate(90f, 180f, 0f);
    }
}
