using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

public sealed class GameControllerInspectionState : GameControllerState
{
    [Inject] private InspectorController _inspectorController;

    protected override ControllerState State => ControllerState.Inspecting;
    
    private InputAction _pointerPositionAction;
    private InputAction _inspectionAction;
    
    public override void Initialize(InputActionMap actionMap)
    {
        _pointerPositionAction = actionMap["PointerPosition"];
        _inspectionAction = actionMap["Inspection"];
        
        _inspectorController.InspectionStopped += InvokeTryExitState;
        _inspectionAction.performed += TryEnterState;
    }

    private void TryEnterState(InputAction.CallbackContext context)
    {
        InvokeTryEnterState();
    }

    protected override void OnEnter()
    {
        _inspectorController.TryToStartInspecting(GetPointerPosition());
    }

    protected override void OnExit()
    {
        _inspectorController.StopInspecting();
    }

    public override bool CanEnterStateFrom(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Inspecting;

    public override bool CanExitStateTo(ControllerState currentState) => currentState is ControllerState.Idle or ControllerState.Inspecting or ControllerState.Dragging or ControllerState.Rotating or ControllerState.PositionDragging;
    
    private void FixedUpdate() 
    {
        if (_inspectorController.TryFindIdleInspectable(GetPointerPosition(), out InspectableObject inspectable))
        {
            if (inspectable.IsInspected) return;
            
            if (inspectable.IdleInspectionDelay > 0) TryIdleInspect(inspectable).Forget();
            else InvokeTryEnterState();
        }
    }

    private async UniTask TryIdleInspect(InspectableObject inspectable)
    {
        inspectable.SetInspectedState(true);
        
        float elapsedTime = 0f;

        while (elapsedTime < inspectable.IdleInspectionDelay)
        {
            if (!_inspectorController.TryFindIdleInspectable(GetPointerPosition(), out InspectableObject foundInspectable) ||
                foundInspectable != inspectable)
            {
                break;
            }
            
            await UniTask.WaitForFixedUpdate();
            
            elapsedTime += Time.unscaledDeltaTime;
        }
        
        inspectable.SetInspectedState(false);

        if (elapsedTime >= inspectable.IdleInspectionDelay)
        {
            InvokeTryEnterState();
        }
    }
    
    private Vector2 GetPointerPosition() => _pointerPositionAction.ReadValue<Vector2>();
}
