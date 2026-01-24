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

        protected void OnEnable() => SetState(true);
        
        protected void OnDisable() => SetState(false);
        
        private async UniTask StartLifetimeTrack()
        {
            StopLifetimeTrack();
            
            _cancellationTokenSource = new();
            _lifetimeTrackActive = true;
            
            try
            { 
                await UniTask.WaitForSeconds(_lifetime, cancellationToken: _cancellationTokenSource.Token); 
                SetState(false);
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
        
        protected void SetState(bool state)
        {
            if (!gameObject) return;
            
            Collider.enabled = state;
            gameObject.SetActive(state);
            
            if (state == gameObject.activeSelf) return;
            
            if (state) StartLifetimeTrack().Forget();
            else StopLifetimeTrack();
        }

        private void OnDestroy() => StopLifetimeTrack();
        
        #endregion
    }
}