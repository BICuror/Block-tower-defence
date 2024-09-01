using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSelectionOption", menuName = "Selection/UpgradeSelectionOption")]

public class UpgradeSelectionOption : SelectionOption
{
    [SerializeField] private Upgrade _uprgade;
    public Upgrade UpgradeInSelectionOption => _uprgade;

    [SerializeField] private GameObject _inspectableObject;
    public GameObject Inspectable => _inspectableObject;
}
