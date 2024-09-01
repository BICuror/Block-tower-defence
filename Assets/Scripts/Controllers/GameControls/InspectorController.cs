using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]

public class InspectorController : MonoBehaviour
{
    [SerializeField] private LayerSetting _inspectableLayerSetting;

    private InspectableObject _inspectable;
    private bool _lastSeenOnInspectable;

    public UnityEvent<InspectableObject> InspectionStarted;
    public UnityEvent InspectionEnded;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    public void TryToStartInspecting(Vector2 mousePosition)
    {
        bool isOnInspectable = IsOnInspectable(mousePosition);

        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (isOnInspectable && _lastSeenOnInspectable == false)
        {
            StartInspecting(mousePosition);
        }
        else if (isOnInspectable && Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _inspectableLayerSetting.GetLayerMask()))
        {
            if (_inspectable != null && rayInfo.collider.gameObject.GetComponent<InspectableObject>() != _inspectable) StartInspecting(mousePosition);
        }
        else if (_lastSeenOnInspectable && isOnInspectable == false)
        {
            StopInspecting();
        }

        _lastSeenOnInspectable = isOnInspectable;
    }

    private bool IsOnInspectable(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _inspectableLayerSetting.GetLayerMask()))
        {
            return (rayInfo.collider.gameObject.TryGetComponent(out InspectableObject inspectable));
        }

        return false;
    }

    public void StartInspecting(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _inspectableLayerSetting.GetLayerMask()))
        {
            if (rayInfo.collider.gameObject.TryGetComponent(out InspectableObject inspectable))
            {
                _inspectable = inspectable;
                InspectionStarted.Invoke(_inspectable);
            }
        }
    }

    public void TryToStopInspecting()
    {
        if (_inspectable != null) StopInspecting();
    }

    private void StopInspecting()
    {
        InspectionEnded.Invoke();

        _inspectable = null;
    }
}
