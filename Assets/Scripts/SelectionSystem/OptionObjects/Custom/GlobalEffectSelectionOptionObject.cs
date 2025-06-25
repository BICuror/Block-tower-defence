using Cysharp.Threading.Tasks;
using Zenject;

public sealed class GlobalEffectSelectionOptionObject : SelectionOptionObject
{
    [Inject] private GlobalEffectContainer _globalEffectContainer;
 
    private ToggleGlobalEffectData _toggleGlobalEffectData;

    public override string OptionName => _toggleGlobalEffectData.EffectName;
    public override string OptionDescription => _toggleGlobalEffectData.EffectDescription;

    public override void ApplyEffect()
    {
        _globalEffectContainer.AddEffect(_toggleGlobalEffectData);
    }

    public void SetGlobalEffectData(ToggleGlobalEffectData data)
    {
        _toggleGlobalEffectData = data;
    }
}