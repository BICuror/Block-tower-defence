using UnityEngine;

public sealed class TestGlobalToggleEffect : GlobalToggleEffect
{
    public override void Enable()
    {
        Debug.Log("Enabled effect");
    }

    public override void Disable()
    {
        Debug.Log("Disabled effect");
    }
}
