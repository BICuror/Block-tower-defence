using Zenject;

public sealed class StartBuildingSelectionGlobalRewardEffect : GlobalRewardEffect
{
    [Inject] private SelectionManager _selectionManager;
    
    public override void GrantReward()
    {
        _selectionManager.EnqeueSelection(new SelectionSettings(SelectionType.Building));
    }
}