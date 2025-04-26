namespace Combat
{
    public sealed class WorkFasterEffect : EntityEffect
    {
        private const float WORK_TIME_MODIFIER_DECREASE = 0.3f;

        protected override void OnInitialized()
        {
            throw new System.NotImplementedException();
        }

        public override void Update() {}

        public override void ApplyToEntity()
        {
            if (Entity.StatContainer.TryGetComponent<TaskRechargeDuration>(out TaskRechargeDuration taskRechargeDuration))
            {
                taskRechargeDuration.ChangeMultiplier(-WORK_TIME_MODIFIER_DECREASE);
            }
        }

        public override void RemoveFromEntity()
        {
            if (Entity.StatContainer.TryGetComponent<TaskRechargeDuration>(out TaskRechargeDuration taskRechargeDuration))
            {
                taskRechargeDuration.ChangeMultiplier(WORK_TIME_MODIFIER_DECREASE);
            }
        }
    }
}