using Cashing;
using System;
using Cysharp.Threading.Tasks;

namespace Combat
{
    public abstract class CombatEntity : EntityComponentCacher, IActivatable
    {
        private const float INVANURABILITY_PERIOD = 0.3f;
        
        private EntityValueModifierContainer _valueModifierContainer;

        public EntityValueModifierContainer ValueModifierContainer => _valueModifierContainer;
        public DraggableObject Draggable => ComponentsContainer.Get<DraggableObject>();
        public EntityHealth Health => ComponentsContainer.Get<EntityHealth>();

        public event Action Activated;

        protected void Awake()
        {
            base.Awake();
            
            _valueModifierContainer = new EntityValueModifierContainer(this);
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