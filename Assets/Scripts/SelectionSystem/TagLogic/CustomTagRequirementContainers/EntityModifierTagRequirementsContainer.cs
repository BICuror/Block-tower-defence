using System.Collections.Generic;
using System;

namespace Combat.Entity.Modificators
{
    [Serializable]
    public sealed class EntityModifierTagRequirementsContainer
    {
        public List<EntityEffectTagReqirement> Requirements;
    }

    [Serializable]
    public sealed class EntityEffectTagReqirement
    {
        public EntityModifcatorTag Tag;
        public int Amount = 1;
    }
}