namespace Combat
{
    public class EnemyWeapon : Weapon<BuildingEntity> 
    {
        private BuildingEntity _targetBuilding;
    
        public void SetTargetBuilding(BuildingEntity buildingHealth) => _targetBuilding = buildingHealth;
    
        protected override bool IsSutableTarget(BuildingEntity buildingHealth) => buildingHealth == _targetBuilding;
    }
}
