using Zenject;

public sealed class GlobalEffectSelectionOptionObject : SelectionOptionObject
{
    [Inject] private GlobalEffectContainer _globalEffectContainer;
 
    private ToggleEffectData _toggleEffectData;
    
    public override void ApplyEffect()
    {
        _globalEffectContainer.AddEffect(_toggleEffectData);
    }

    public void SetGlobalEffectData(ToggleEffectData data)
    {
        _toggleEffectData = data;
    }
}