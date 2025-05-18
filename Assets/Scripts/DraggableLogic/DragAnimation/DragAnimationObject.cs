using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public sealed class DragAnimationObject : MonoBehaviour
{
    [SerializeField] private bool _returnToDefaultYRotation = false;
    
    private float _heightDistance;
    private Transform _initialParent;
    private Vector3 _initialLocalPosition;
    private float _initialLocalYRotation;
    public Vector3 InitialLocalPosition => _initialLocalPosition;
    
    private void Awake()
    {
        _heightDistance = GetComponent<MeshRenderer>().bounds.size.y;
        _initialParent = transform.parent;
        _initialLocalPosition = transform.localPosition;
        _initialLocalYRotation = transform.localRotation.eulerAngles.y;
    }

    private void OnEnable()
    {
        SetInitialParent();
    }

    public void ConnectToJoint(Joint joint)
    {
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        
        transform.position = joint.transform.position - new Vector3(0f, _heightDistance, 0f);

        rigidbody.mass = 5f;
        transform.parent = null;
        rigidbody.useGravity = true;

        rigidbody.constraints = RigidbodyConstraints.None; 

        joint.connectedBody = rigidbody;
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
    }
}