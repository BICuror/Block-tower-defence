using UnityEngine;
using System;

public sealed class DragAnimationObject : MonoBehaviour
{
    [SerializeField] private bool _returnToDefaultYRotation = false;
    
    private float _defaultScale = 1f;
    private Transform _initialParent;
    private Vector3 _initialLocalPosition;
    private float _initialLocalYRotation;
    private bool _isDragged;
    
    public Vector3 InitialLocalPosition => _initialLocalPosition;
    public bool IsDragged => _isDragged;
    public float MeshHeight => 1f;
    
    public event Action<float> DefaultScaleChanged;
    public event Action DragStarted;
    public event Action DragEnded;
    
    private void Awake()
    {
        _initialParent = transform.parent;
        _initialLocalPosition = transform.localPosition;
        _initialLocalYRotation = transform.localRotation.eulerAngles.y;
    }

    private void OnEnable() => SetInitialParent();

    public void SetDefaultScale(float scale)
    {
        _defaultScale = scale;
        
        DefaultScaleChanged?.Invoke(scale);
    }
    
    public void ConnectToJoint(Joint joint)
    {
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        
        transform.position = joint.transform.position - new Vector3(0f, MeshHeight, 0f);

        transform.parent = null;
        rigidbody.mass = 5f;
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None; 

        joint.connectedBody = rigidbody;

        _isDragged = true;

        DragStarted?.Invoke();
    } 

    public void DisconnectFromJoint(Joint joint)
    {
        Destroy(GetComponent<Rigidbody>());
        joint.connectedBody = null;
        
        DragEnded?.Invoke();
    }

    public void SetInitialParent()
    {
        transform.parent = _initialParent;
        transform.localPosition = _initialLocalPosition;
        if (_returnToDefaultYRotation) transform.localRotation = Quaternion.Euler(0f, _initialLocalYRotation, 0f);
        
        _isDragged = false;
    }
}