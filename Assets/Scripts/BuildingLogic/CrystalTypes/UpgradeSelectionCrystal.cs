using UnityEngine;

public class UpgradeSelectionCrystal : SelectionCrystal
{
    [SerializeField] private SelectionOptionContainer _selectionOptionContainer;
    public override SelectionOptionContainer GetOptionContainer() => _selectionOptionContainer;

    protected override void ApplySelectionOption(SelectionOption selectionOption)
    {
        UpgradeSelectionOption option = selectionOption as UpgradeSelectionOption;   

        option.UpgradeInSelectionOption.Apply();
    }
}
