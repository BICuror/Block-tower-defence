public sealed class BuildingEffectManager : EntityEffectManager 
{
    private void Awake()
    {
        Building building = GetComponent<Building>();
        
        building.PickedUp.AddListener(RemoveAllEffects);
    
        building.PickedUp.AddListener(SetEffectsCanBeSetFalse);
        building.Placed.AddListener(SetEffectsCanBeSetTrue);
    }

    private void SetEffectsCanBeSetTrue() => _effectsCanBeSet = true;
    private void SetEffectsCanBeSetFalse() => _effectsCanBeSet = false;
}
