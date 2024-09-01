using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private float _height = 1f;

    public float GetInpectionPanelInstallationHeight() => _height;

    public void SetName(string name) => _name = name;
    public void SetDescription(string description) => _description = description;
}
