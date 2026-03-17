using TMPEffects.SerializedCollections;
using UnityEngine;

public sealed class CursorController : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<CursorState, Texture2D> _cursorStateSprites;
    [SerializeField] private CursorMode _cursorMode = CursorMode.Auto;
    private CursorState _cursorState;
    
    public CursorState State => _cursorState;
    
    private void Start() => SetCursorState(CursorState.Idle);

    public void SetCursorState(CursorState cursorState)
    {
        _cursorState = cursorState;
        
        Cursor.SetCursor(_cursorStateSprites[_cursorState], Vector2.zero, _cursorMode);
    }
        
    public enum CursorState
    {
        Idle,
        Drag,
        Move,
        InspectionAvailable
    }
}