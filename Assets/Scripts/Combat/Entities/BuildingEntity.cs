using Cashing;

namespace Combat
{
    public class BuildingEntity : CombatEntity
    {
        [Cached] private BuildTime _buildTime;
    }
}