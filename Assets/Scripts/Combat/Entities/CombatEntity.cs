using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using System;

namespace Combat
{
    public abstract class CombatEntity : EntityComponentCacher, IActivatable
    {
        private const float INVANURABILITY_PERIOD = 0.3f;

        [Header("DeathLogic")] 
        [SerializeField] private EntityDeathHandleMethod _deathHandleHandleMethod = EntityDeathHandleMethod.Disable;
        
        private EntityValueModifierContainer _valueModifierContainer;

        public EntityDeathHandleMethod DeathHandleMethod => _deathHandleHandleMethod;
        public EntityValueModifierContainer ValueModifierContainer => _valueModifierContainer;
        public EntityHealth Health => ComponentsContainer.Get<EntityHealth>();

        public event Action Activated;

        protected void Awake()
        {
            base.Awake();
            
            _valueModifierContainer = new EntityValueModifierContainer(this);
            
            Health.HandleDeath += HandleDeath;
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

        private void HandleDeath()
        {
            switch (_deathHandleHandleMethod)
            {
                case EntityDeathHandleMethod.Disable:
                {
                    gameObject.SetActive(false); 
                } break;
                case EntityDeathHandleMethod.Destroy:
                {
                    Destroy(gameObject);
                    
                    Health.HandleDeath -= HandleDeath;
                } break;
                case EntityDeathHandleMethod.None: break;
            }

            OnHandleDeath();
        }

        protected virtual void OnHandleDeath() {}
    }

    public enum EntityDeathHandleMethod
    {
        None,
        Disable,
        Destroy,
    }
}