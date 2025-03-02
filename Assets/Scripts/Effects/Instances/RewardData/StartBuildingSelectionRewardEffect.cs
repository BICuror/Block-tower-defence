using Zenject;

public sealed class StartBuildingSelectionRewardEffect : RewardEffect
{
    [Inject] private SelectionManager _selectionManager;
    
    public override void GrantReward()
    {
        _selectionManager.EnqeueSelection(new SelectionSettings(SelectionType.Building));
    }
}