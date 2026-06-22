using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace GameControls.Features
{
    public sealed class HoverableGameControllerFeature : GameControllerFeature
    {
        [SerializeField] private LayerSetting _hoverableLayerSetting;
        [SerializeField] private Camera _camera;

        private List<HoverableObject> _lastHoveredOverObjects = new();
        private InputAction _pointerPositionAction;
        
        public override void Initialize()
        {
            _pointerPositionAction = InputActionMap["PointerPosition"];
        }

        protected override void OnEnableFeature() {}

        protected override void OnDisableFeature() => _lastHoveredOverObjects.Clear();
        
        private void FixedUpdate()
        {
            CheckHover(_pointerPositionAction.ReadValue<Vector2>());
        }
        
        public void CheckHover(Vector2 pointerPosition)
        {
            Ray cameraRay = _camera.ScreenPointToRay(pointerPosition);

            RaycastHit[] hits = Physics.RaycastAll(cameraRay, TileMap.RAY_LENGTH, _hoverableLayerSetting.GetLayerMask());

            List<HoverableObject> hoveredOverObjects = new();

            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.collider.gameObject.TryGetComponent(out HoverableObject hoverableObject))
                {
                    hoveredOverObjects.Add(hoverableObject);
                }
            }

            for (int i = 0; i < _lastHoveredOverObjects.Count;)
            {
                if (hoveredOverObjects.Contains(_lastHoveredOverObjects[i])) i++;
                else
                {
                    _lastHoveredOverObjects[i].ExitHover();
                    _lastHoveredOverObjects.RemoveAt(i);
                }
            }

            foreach (HoverableObject hoverableObject in hoveredOverObjects)
            {
                if (!_lastHoveredOverObjects.Contains(hoverableObject))
                {
                    _lastHoveredOverObjects.Add(hoverableObject);
                    hoverableObject.EnterHover();
                }
            }
        }
    }
}