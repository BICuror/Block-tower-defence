using UnityEngine;

public class Inspectable : MonoBehaviour
{
    [SerializeField] private string _inspectableObjectName;
    [SerializeField] private string _inspectableObjectDescription;
    
    public string Name => _inspectableObjectName;
    public string Description => _inspectableObjectDescription;
}

public enum InspectableType
{
    Building,
    Enemy
}