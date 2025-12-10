using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class Weapon : WeaponBase
    {
        [SerializeField] protected Collider Collider; 
        [SerializeField] protected Rigidbody Rigidbody;
        
        private CancellationTokenSource _cancellationTokenSource = new();
        private float _lifetime;
        private bool _lifetimeTrackActive;
        
        public void Initialize(CombatEntity ownerEntity, float lifetime)
        {
            OwnerEntity = ownerEntity;
            _lifetime = lifetime;
            OnInitialized();
        }
        
        #region StateManagements

        protected void OnEnable() => Enable();
        protected void OnDisable() => StopLifetimeTrack();
        
        public void Enable()
        {
            StopLifetimeTrack();
            StartLifetimeTrack();
            SetState(true);
        }
        
        public void Disable()
        {
            SetState(false);
        }
        
        private async UniTask StartLifetimeTrack()
        {
            _cancellationTokenSource = new();
            _lifetimeTrackActive = true;
            
            try
            { 
                await UniTask.WaitForSeconds(_lifetime, cancellationToken: _cancellationTokenSource.Token); 
                Disable();
            }
            catch (Exception e) { e.LogAsync(); }

            _lifetimeTrackActive = false;
        }

        private void StopLifetimeTrack()
        {
            if (!_lifetimeTrackActive) return;
            
            _cancellationTokenSource.Cancel();
            _lifetimeTrackActive = false;
        }
        
        private void SetState(bool state)
        {
            if (!gameObject) return;
            
            Collider.enabled = state;
            gameObject.SetActive(state);
        }

        private void OnDestroy() => StopLifetimeTrack();
        
        #endregion
    }
}