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
        
        public Rigidbody RB => Rigidbody;
        
        public void Initialize(CombatEntity ownerEntity, float lifetime)
        {
            base.Initialize(ownerEntity);
            _lifetime = lifetime;
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

        private void OnDestroy() => StopLifetimeTrack();
        
        #endregion
    }
}