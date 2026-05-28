using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class Weapon : ColliderWeapon
    {
        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _lifetimeTrackActive;
        private float _lifetime;
        
        public void Initialize(CombatEntity ownerEntity, float lifetime)
        {
            OwnerEntity = ownerEntity;
            _lifetime = lifetime;
            OnInitialized();
        }
        
        #region StateManagements

        protected void OnEnable() => StartLifetimeTrack().Forget();

        protected void OnDisable() => StopLifetimeTrack();
        
        private async UniTask StartLifetimeTrack()
        {
            _cancellationTokenSource = new();

            _lifetimeTrackActive = true;
            
            try
            {
                await UniTask.WaitForSeconds(_lifetime, cancellationToken: _cancellationTokenSource.Token, cancelImmediately: true);
            }
            catch (Exception e)
            {
                e.LogAsync(); 
            }

            _lifetimeTrackActive = false;
            
            SetState(false);
        }

        public void StopLifetimeTrack()
        {
            if (!_lifetimeTrackActive) return;
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        #endregion
    }
}