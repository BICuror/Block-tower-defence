using System.Collections.Generic;
using System;

[Serializable] public sealed class EntityModifierTagRequirementsContainer
{
    public List<EntityEffectTagReqirement> Requirements;
}

[Serializable] public sealed class EntityEffectTagReqirement
{
    public EntityModifcatorTag Tag;
    public int Amount = 1;
}