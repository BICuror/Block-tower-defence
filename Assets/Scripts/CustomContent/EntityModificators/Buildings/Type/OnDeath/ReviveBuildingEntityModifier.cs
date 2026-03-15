using Cysharp.Threading.Tasks;
using Combat;

public sealed class ReviveBuildingEntityModifier : EntityModificator
{
    private BuildingHealth _buildingHealth;
    private float _reviveDuration;
    
    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<BuildingHealth>();
    
    public override void Enable()
    {
        _buildingHealth = Entity.ComponentsContainer.Get<BuildingHealth>();
        _reviveDuration = Args.GetArgument<float>("ReviveDuration");

        _buildingHealth.Died += StartReviveProcess;
    }

    private void StartReviveProcess() => ReviveEntity().Forget();

    private async UniTask ReviveEntity()
    {
        await UniTask.WaitForSeconds(_reviveDuration);

        if (!Entity.Health.IsAlive())
        {
            Entity.ComponentsContainer.Get<BuildingEntity>().ReviveBuilding().Forget();
        }
    }
    
    public override void Disable()
    {
        _buildingHealth.Died -= StartReviveProcess;
    }
}