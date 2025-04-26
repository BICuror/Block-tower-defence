using Zenject;

public sealed class StartGlobalEffectSelectionGlobalRewardEffect : GlobalRewardEffect
{
    [Inject] private SelectionManager _selectionManager;
    
    public override void GrantReward()
    {
        _selectionManager.EnqeueSelection(new SelectionSettings(SelectionType.GlobalEffect));
    }
}