public sealed class GameControllerIdleState : GameControllerState
{
    protected override ControllerState State => ControllerState.Idle;
    public override void Initialize(GameControls controls) {}

    public override bool CanEnterStateFrom(ControllerState currentState) => true;
    public override bool CanExitStateTo(ControllerState currentState) => true;
}