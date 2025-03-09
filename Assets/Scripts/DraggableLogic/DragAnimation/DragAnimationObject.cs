using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]

public sealed class DragAnimationObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _heightDistance;
    private Vector3 _initialLocalPosition;
    private Transform _initialParent;

    public Vector3 InitialLocalPosition => _initialLocalPosition;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _heightDistance = GetComponent<MeshRenderer>().bounds.size.y;
        _initialLocalPosition = transform.localPosition;
        _initialParent = transform.parent;
    }
    
    public void ConnectToJoint(Joint joint)
    {
        transform.position = joint.transform.position - new Vector3(0f, _heightDistance, 0f);

        transform.parent = null;
        
        _rigidbody.useGravity = true;

        _rigidbody.constraints = RigidbodyConstraints.None; 

        joint.connectedBody = _rigidbody;
    } 

    public void DisconnectFromJoint(Joint joint)
    {
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        joint.connectedBody = null;
        transform.parent = null;
    }

    public void SetInitialParent()
    {
        transform.parent = _initialParent;
        transform.localPosition = _initialLocalPosition;
    }
}