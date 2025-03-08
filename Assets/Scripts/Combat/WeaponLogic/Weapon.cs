using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected Collider Collider; 
        [SerializeField] protected Rigidbody Rigidbody;
        protected CombatEntity OwnerEntity;
        
        private CancellationTokenSource _cancellationTokenSource = new();
        private float _lifetime;
        private bool _lifetimeTrackActive;
        
        public Rigidbody RB => Rigidbody;
    
        public Action HitEntity;
        public Action KilledEntity;
        
        public void Initialize(CombatEntity ownerEntity, float lifetime)
        {
            OwnerEntity = ownerEntity;
            _lifetime = lifetime;

            OnInitialized();
        }

        protected virtual void OnInitialized() {}
    
        protected void DamageEntity(float damageAmount, CombatEntity receivingEntity)
        {
            if (!receivingEntity.Health.IsAlive()) return;
            
            float multipliedAttackDamage = OwnerEntity.DamageModifierContainer.DealerContainer.Modify(damageAmount, receivingEntity);
            
            receivingEntity.Health.ReceiveEnemyDamage(multipliedAttackDamage, OwnerEntity);
            
            HitEntity?.Invoke();
            
            if (!receivingEntity.Health.IsAlive()) KilledEntity?.Invoke();
        }
        
        #region StateManagements

        private void OnEnable() => Enable();
        
        public void Enable()
        {
            StopLifetimeTrack();
            StartLifetimeTrack();
            SetState(true);
        }
        
        protected void Disable()
        {
            StopLifetimeTrack();
            SetState(false);
        }
        
        private async UniTask StartLifetimeTrack()
        {
            _lifetimeTrackActive = true;
            
            try
            { 
                await UniTask.WaitForSeconds(_lifetime, cancellationToken: _cancellationTokenSource.Token); 
                Disable();
            }
            catch (Exception e) { TaskUtility.LogAsync(e); }

            _lifetimeTrackActive = false;
        }

        private void StopLifetimeTrack()
        {
            if (!_lifetimeTrackActive) return;
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
            _lifetimeTrackActive = false;
        }
        
        private void SetState(bool state)
        {
            Collider.enabled = state;
            gameObject.SetActive(state);
        }
        #endregion
    }
}