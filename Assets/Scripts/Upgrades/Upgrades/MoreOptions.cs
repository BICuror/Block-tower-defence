using UnityEngine;

[CreateAssetMenu(fileName = "MoreOptions", menuName = "Upgrades/MoreOptions")]

public sealed class MoreOptions : Upgrade
{
    public override void Apply()
    {
        FindObjectOfType<SelectionManager>().SetSelectionOptionAmount(1);
    }
}
