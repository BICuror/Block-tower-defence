public class EnemyWeapon : Weapon<BuildingHealth> 
{
    private BuildingHealth _targetBuilding;

    public void SetTargetBuilding(BuildingHealth buildingHealth) => _targetBuilding = buildingHealth;

    protected override bool IsSutableTarget(BuildingHealth buildingHealth) => buildingHealth == _targetBuilding;
}