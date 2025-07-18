namespace Combat
{
    public sealed class WorkFasterEffect : EntityEffect
    {
        private StatModifier _statModifier = new StatModifier();
        
        public override EntityEffectType EffectType => EntityEffectType.Positive;
        public override bool CanBeApplied() => Entity.ComponentsContainer.Has<TaskCycle>();
        
        protected override void OnInitialized()
        {
            _statModifier.SetFlat(ArgumentsContainer.GetArgument<float>("RechargeSpeedIncrease"));
        }

        public override void ApplyToEntity()
        {
            Entity.StatContainer.Get<TaskRechargeDuration>().AddStatModifier(_statModifier);
        }

        public override void RemoveFromEntity()
        {
            Entity.StatContainer.Get<TaskRechargeDuration>().RemoveStatModifier(_statModifier);
        }
    }
}