namespace Combat
{
    public sealed class WorkFasterEffect : EntityEffect
    {
        private const float WORK_TIME_MODIFIER_DECREASE = -0.3f;
        private StatModifier _statModifier = new StatModifier();
        
        public override bool CanBeApplied() => Entity.ComponentsContainer.Has<TaskCycle>();
        
        protected override void OnInitialized()
        {
            _statModifier.SetFlat(WORK_TIME_MODIFIER_DECREASE);
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