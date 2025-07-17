using System.Collections.Generic;
using System.Linq;

public sealed class EntityTagRequirementsChecker
{
    public static bool RequirementsAreMet(List<EntityModifcatorTag> appliedTags, List<EntityEffectTagReqirement> reqirements)
    {
        for (int i = 0; i < reqirements.Count; i++)
        {
            if (appliedTags.Count(tag => tag == reqirements[i].Tag) < reqirements[i].Amount) return false;
        }

        return true;
    }
}