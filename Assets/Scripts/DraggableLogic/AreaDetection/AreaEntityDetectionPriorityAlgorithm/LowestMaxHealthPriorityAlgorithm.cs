using System.Collections.Generic;
using Combat;

public sealed class LowestMaxHealthPriorityAlgorithm : EntityDetectorPriorityAlgorithm
{
    public override CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList)
    {
        float minHealth = float.MaxValue;
        CombatEntity bestCandidate = null;
            
        for (int i = 0; i < initialList.Count; i++)
        {
            float maxHp = initialList[i].Health.GetMaxHp();
                
            if (maxHp < minHealth)
            {
                minHealth = maxHp;
                bestCandidate = initialList[i];
            }
        }
            
        return bestCandidate;
    }
}