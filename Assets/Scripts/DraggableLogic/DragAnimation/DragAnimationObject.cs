using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public sealed class DragAnimationObject : MonoBehaviour
{
    [SerializeField] private bool _returnToDefaultYRotation = false;
    
    private float _meshHegiht;
    private Transform _initialParent;
    private Vector3 _initialLocalPosition;
    private float _initialLocalYRotation;
    private bool _isConnected;
    
    public Vector3 InitialLocalPosition => _initialLocalPosition;
    public bool IsConnected => _isConnected;
    public float MeshHeight => _meshHegiht;
    
    private void Awake()
    {
        _meshHegiht = GetComponent<MeshRenderer>().bounds.size.y;
        _initialParent = transform.parent;
        _initialLocalPosition = transform.localPosition;
        _initialLocalYRotation = transform.localRotation.eulerAngles.y;
    }

    private void OnEnable() => SetInitialParent();

    public void ConnectToJoint(Joint joint)
    {
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        
        transform.position = joint.transform.position - new Vector3(0f, _meshHegiht, 0f);

        transform.parent = null;
        rigidbody.mass = 5f;
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None; 

        joint.connectedBody = rigidbody;

        _isConnected = true;
    } 

    public void DisconnectFromJoint(Joint joint)
    {
        Destroy(GetComponent<Rigidbody>());
        joint.connectedBody = null;
    }

    public void SetInitialParent()
    {
        transform.parent = _initialParent;
        transform.localPosition = _initialLocalPosition;
        if (_returnToDefaultYRotation) transform.localRotation = Quaternion.Euler(0f, _initialLocalYRotation, 0f);
        
        _isConnected = false;
    }
}