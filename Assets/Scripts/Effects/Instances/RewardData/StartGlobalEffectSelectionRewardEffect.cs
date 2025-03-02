using Zenject;

public sealed class StartGlobalEffectSelectionRewardEffect : RewardEffect
{
    [Inject] private SelectionManager _selectionManager;
    
    public override void GrantReward()
    {
        _selectionManager.EnqeueSelection(new SelectionSettings(SelectionType.GlobalEffect));
    }
}