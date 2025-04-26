using UnityEngine;

namespace Combat
{
    public sealed class SlowdownEffect : EntityEffect
    {
        private const float TIME_PER_BLOCK_TRAVELED_INCREASED_PER_STACK = 0.25f;
        private StatModifier _statModifier;
        
        protected override void OnInitialized()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            _statModifier.FlatModifier = TIME_PER_BLOCK_TRAVELED_INCREASED_PER_STACK * Stack;
        }

        public override void ApplyToEntity()
        {
            _statModifier = new StatModifier();
            
            Entity.StatContainer.Get<Speed>().AddStatModifier(_statModifier);
        }

        public override void RemoveFromEntity()
        {
            Entity.StatContainer.Get<Speed>().RemoveStatModifier(_statModifier);
        }
    }
}