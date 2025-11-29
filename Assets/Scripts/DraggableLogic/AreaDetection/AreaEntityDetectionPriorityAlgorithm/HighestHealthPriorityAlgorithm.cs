using System.Collections.Generic;
using Combat;

public sealed class HighestHealthPriorityAlgorithm : EntityDetectorPriorityAlgorithm
{
    public override CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList)
    {
        float maxHealth = 0f;
        CombatEntity bestCandidate = null;
            
        for (int i = 0; i < initialList.Count; i++)
        {
            float hpPercent = initialList[i].Health.GetHpPercent();

            if (hpPercent == 1f) return initialList[i];
                
            if (hpPercent > maxHealth)
            {
                maxHealth = hpPercent;
                bestCandidate = initialList[i];
            }
        }
            
        return bestCandidate;
    }
}