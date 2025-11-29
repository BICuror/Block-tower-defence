using System.Collections.Generic;
using Combat;

public sealed class HighestMaxHealthPriorityAlgorithm : EntityDetectorPriorityAlgorithm
{
    public override CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList)
    {
        float maxHealth = 1;
        CombatEntity bestCandidate = null;
            
        for (int i = 0; i < initialList.Count; i++)
        {
            float maxHp = initialList[i].Health.GetMaxHp();
                
            if (maxHp > maxHealth)
            {
                maxHealth = maxHp;
                bestCandidate = initialList[i];
            }
        }
            
        return bestCandidate;
    }
}