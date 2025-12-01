using System.Collections.Generic;
using System.Linq;

public sealed class GlobalEffectTagRequirementsChecker
{
    public static bool RequirementsAreMet(List<GlobalEffectData> globalEffectDatas, List<GlobalEffectTagReqirement> requirements)
    {
        List<GlobalEffectTag> appliedTags = new List<GlobalEffectTag>();
        
        globalEffectDatas.ForEach(effectData => 
        {
            effectData.Tags.ForEach(tag => appliedTags.Add(tag));
        });
        
        for (int i = 0; i < requirements.Count; i++)
        {
            if (appliedTags.Count(tag => tag == requirements[i].Tag) < requirements[i].Amount) return false;
        }

        return true;
    }
}
