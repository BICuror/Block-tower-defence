using TMPEffects.SerializedCollections;
using UnityEngine.InputSystem;
using GameControls.Features;
using GameControls.States;
using System.Linq;
using UnityEngine;

namespace GameControls
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<ControllerFeature, GameControllerFeature> _features = new();
        [SerializeField] private SerializedDictionary<ControllerState, GameControllerState> _states = new();
        [SerializeField] private PlayerInput _controls;

        private ControllerState _currentControllerState;
        
        #region Enable\Disable
        
        private void Awake() => CreateControls();
        
        private void CreateControls()
        {
            InitializeStates();
            InitializeFeatures();
        }
        
        public void Enable() => _controls.ActivateInput();
        public void Disable() => _controls.DeactivateInput();

        public void Dispose()
        {
            _features.Keys.ToList().ForEach(DisableFeature);
            _states.Keys.ToList().ForEach(DisableState);

            _controls.currentActionMap.Disable();
            _controls.currentActionMap.Dispose();
        }
        
        #endregion

        #region Features
        
        public void EnableFeature(ControllerFeature feature) => _features[feature].EnableFeature();
        public void DisableFeature(ControllerFeature feature) => _features[feature].DisableFeature();
        
        private void InitializeFeatures()
        {
            foreach (GameControllerFeature gameControllerFeature in _features.Values)
            {
                gameControllerFeature.SetInputActionMap(_controls.currentActionMap);
                gameControllerFeature.Initialize();
                gameControllerFeature.EnableFeature();
            }
        }

        #endregion
        
        #region States
        
        public void EnableState(ControllerState state) => _states[state].EnableState();
        public void DisableState(ControllerState state) => _states[state].DisableState();
        
        private void InitializeStates()
        {
            foreach (GameControllerState gameControllerState in _states.Values)
            {
                gameControllerState.SetInputActionMap(_controls.currentActionMap);
                gameControllerState.Initialize();
                gameControllerState.EnableState();
                gameControllerState.TriedToEnterState += TryEnterState;
                gameControllerState.TriedToExitState += TryExitState;
            }
        }

        private void TryEnterState(ControllerState state)
        {
            if (_states[_currentControllerState].CanExitStateTo(state) &&
                _states[state].CanEnterStateFrom(_currentControllerState))
            {
                _states[_currentControllerState].Exit();
                _currentControllerState = state;
                _states[state].Enter();
            }
        }

        private void TryExitState(ControllerState state)
        {
            if (_currentControllerState != state) return;

            ControllerState idleState = ControllerState.Idle;

            if (_states[_currentControllerState].CanExitStateTo(idleState) &&
                _states[idleState].CanEnterStateFrom(_currentControllerState))
            {
                _states[_currentControllerState].Exit();
                _currentControllerState = idleState;
                _states[idleState].Enter();
            }
        }

        #endregion
    }
}