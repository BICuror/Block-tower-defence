namespace Combat
{
    public sealed class BuildingEffectManager : EntityEffectManager 
    {
        private void Awake()
        {
            BuildingDraggable buildingDraggable = GetComponent<BuildingDraggable>();
            
            buildingDraggable.PickedUp += RemoveAllEffects;
        
            buildingDraggable.PickedUp += SetEffectsCanBeSetFalse;
            buildingDraggable.Placed += SetEffectsCanBeSetTrue;
        }
    
        private void SetEffectsCanBeSetTrue() => _effectsCanBeSet = true;
        private void SetEffectsCanBeSetFalse() => _effectsCanBeSet = false;
    }
}