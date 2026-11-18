
using Cysharp.Threading.Tasks;
using Combat.Stats;
using Configs;
using Combat;

namespace ScriptableLogic.SubDomains.TaskCycle
{
    public sealed class EnemyTaskCycleSubDomain : TaskCycleSubDomain
    {
        public override TaskCycleCore GetTaskCycle(TaskRechargeDuration taskRechargeDuration)
        {
            return new EnemyCycleCore((EnemyEntity)Args.Owner, taskRechargeDuration);
        }
    }
    
    public sealed class BuildingTaskCycleSubDomain : TaskCycleSubDomain
    {
        public override TaskCycleCore GetTaskCycle(TaskRechargeDuration taskRechargeDuration)
        {
            return new BuildingCycleCore((BuildingEntity)Args.Owner, taskRechargeDuration);
        }
    }
    
    public abstract class TaskCycleSubDomain : SubDomain
    {
        [Argument("TaskRechargeDuration")] private float _taskRechargeDuration;
        
        private float _damage;
        
        private AreaEntityDetector _areaEntityDetector;
        private TaskCycleCore _taskCycle;
        
        public override UniTask Enable()
        {
            TaskRechargeDuration taskRechargeDurationStat = new TaskRechargeDuration();
            taskRechargeDurationStat.SetDefault(_taskRechargeDuration);

            _taskCycle = GetTaskCycle(taskRechargeDurationStat);
            _taskCycle.Enable();
            
            return UniTask.CompletedTask;
        }
        
        public abstract TaskCycleCore GetTaskCycle(TaskRechargeDuration taskRechargeDuration);
    
        public void TryCycle()
        {
            _taskCycle.TryCycle();
        }
    
        public void TryStopCycle()
        {
            _taskCycle.StopRechargeProcess();
        }
        
        public override UniTask Disable()
        {
            _taskCycle.StopRechargeProcess();
            _taskCycle.Disable();
            
            return UniTask.CompletedTask;
        }
    }

    public sealed class AreaTaskCycleInvoker : SubDomain
    {
        private AreaEntityDetector _areaEntityDetector;
        private TaskCycleSubDomain _taskCycleSubDomain;
        
        public override UniTask Enable()
        {
            _taskCycleSubDomain = Args.Domain.GetSubDomain<TaskCycleSubDomain>();
            
            _areaEntityDetector = Args.Owner.ComponentsContainer.Get<AreaEntityDetector>();

            _areaEntityDetector.AddedItem += TryCycle;
            _areaEntityDetector.RemovedItem += TryStopCycle;
            
            return UniTask.CompletedTask;
        }

        private void TryCycle(CombatEntity _)
        {
            if (_areaEntityDetector.IsEmpty) return;
            
            _taskCycleSubDomain.TryCycle();
        }

        private void TryStopCycle(CombatEntity _)
        {
            if (!_areaEntityDetector.IsEmpty) return;
            
            _taskCycleSubDomain.TryStopCycle();
        }

        public override UniTask Disable()
        {
            _areaEntityDetector.AddedItem -= TryCycle;
            _areaEntityDetector.RemovedItem -= TryStopCycle;
            
            return UniTask.CompletedTask;
        }
    }
}