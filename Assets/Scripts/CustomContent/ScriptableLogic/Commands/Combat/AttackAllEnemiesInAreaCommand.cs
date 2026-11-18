using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.Stats;
using Combat;

namespace ScriptableLogic.Commands.Combat
{
    public sealed class AttackAllEnemiesInAreaCommand : Command, ICommand
    {
        public UniTask Execute()
        {
            IReadOnlyList<CombatEntity> entitiesInArea = Args.Owner.ComponentsContainer.Get<AreaEntityDetector>().GetList();
    
            foreach (CombatEntity combatEntity in entitiesInArea)
            {
                combatEntity.Health.ReceiveEnemyDamage(Args.Owner.StatContainer.Get<Damage>().GetValueModificationContainer(), Args.Owner);
            }
            
            return UniTask.CompletedTask;
        }
    }
}