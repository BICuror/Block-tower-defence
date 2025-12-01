using System.Collections.Generic;
using System;

[Serializable]
public sealed class GlobalEffectTagRequirementContainer
{
    public List<GlobalEffectTagReqirement> Requirements;
}

[Serializable] public sealed class GlobalEffectTagReqirement
{
    public GlobalEffectTag Tag;
    public int Amount = 1;
}