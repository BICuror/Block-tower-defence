using System.Collections.Generic;
using Combat;

public sealed class LowestHealthPriorityAlgorithm : EntityDetectorPriorityAlgorithm
{
    public override CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList)
    {
        float minHealth = 1;
        CombatEntity bestCandidate = null;
            
        for (int i = 0; i < initialList.Count; i++)
        {
            float hpPercent = initialList[i].Health.GetHpPercent();
                
            if (hpPercent < minHealth)
            {
                minHealth = hpPercent;
                bestCandidate = initialList[i];
            }
        }
            
        return bestCandidate;
    }
}