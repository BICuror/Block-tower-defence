using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using Zenject;

namespace Combat.OptionalTasks
{
    public sealed class OptionalTaskObjectsManager
    {
        [Inject] private WaveIndexContainer _waveIndexContainer;
        private List<OptionalTask> _optionalTasks = new();
        
        public void Add(OptionalTask optionalTask)
        {
            _optionalTasks.Add(optionalTask);

            optionalTask.GetComponent<CombatEntity>().Health.EntityDied += RemoveOptionalTaskEntity;
        }

        private void RemoveOptionalTaskEntity(CombatEntity optionalTaskEntity)
        {
            _optionalTasks.Remove(optionalTaskEntity.ComponentsContainer.Get<OptionalTask>());
            
            optionalTaskEntity.Health.EntityDied -= RemoveOptionalTaskEntity;
        }

        public void GrantOptionalTasksRewards()
        {
            foreach (OptionalTask optionalTask in _optionalTasks)
            {
                optionalTask.TryGrantRewards().Forget();
            }
        }
        
        public void UpdateOptionalTasksRewards()
        {
            int rerollsAmount = _waveIndexContainer.GetCurrentWaveContent().Content.Count(type => type == WaveContentType.RerollRewardFromOptionalTask);
    
            for (int i = 0; i < _optionalTasks.Count; i++)
            {
                if (i < rerollsAmount) _optionalTasks[i].SetReward(OptionalTaskRewardType.Reroll);
                else _optionalTasks[i].SetReward(OptionalTaskRewardType.UpgradeCharges);
            }
        }
    }
}