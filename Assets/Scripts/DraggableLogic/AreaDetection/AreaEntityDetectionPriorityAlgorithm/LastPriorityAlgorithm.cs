using System.Collections.Generic;
using Combat;

public sealed class LastPriorityAlgorithm : EntityDetectorPriorityAlgorithm
{
    public override CombatEntity GetPrioritizedEntity(List<CombatEntity> initialList) => initialList[^1];
}