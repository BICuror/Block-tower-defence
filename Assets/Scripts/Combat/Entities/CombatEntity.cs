using Cashing;
using System;
using Cysharp.Threading.Tasks;

namespace Combat
{
    public abstract class CombatEntity : EntityComponentCacher, IActivatable
    {
        private const float INVANURABILITY_PERIOD = 0.3f;
        
        private EntityDamageModifierContainer _damageModifierContainer;

        public EntityDamageModifierContainer DamageModifierContainer => _damageModifierContainer;
        public DraggableObject Draggable => ComponentsContainer.Get<DraggableObject>();
        public EntityHealth Health => ComponentsContainer.Get<EntityHealth>();

        public event Action Activated;

        protected void Awake()
        {
            base.Awake();
            
            _damageModifierContainer = new EntityDamageModifierContainer(this);
        }
        
        public void Activate() => Activated?.Invoke();

        private void OnEnable() => GainAndRemoveInvulnerability();

        private void GainAndRemoveInvulnerability()
        {
            Health.InvulnerabilityTokenContainer.AddToken();
            
            RemoveInvulnerability().Forget();
        }

        private async UniTask RemoveInvulnerability()
        {
            try
            {
                await UniTask.WaitForSeconds(INVANURABILITY_PERIOD, cancellationToken: destroyCancellationToken);
            }
            catch (Exception e)
            {
                e.LogAsync();
                return;
            }
        
            Health.InvulnerabilityTokenContainer.RemoveToken();
        }
    }
}