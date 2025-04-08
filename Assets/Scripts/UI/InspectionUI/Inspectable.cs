using UnityEngine;

public class Inspectable : MonoBehaviour
{
    [SerializeField] private string _inspectableObjectName;
    [SerializeField] private string _inspectableObjectDescription;
    [SerializeField] private InspectableType _inspectableObjectType;
    
    public string Name => _inspectableObjectName;
    public string Description => _inspectableObjectDescription;
    public InspectableType InspectableType => _inspectableObjectType;
}

public enum InspectableType
{
    Building,
    Enemy
}