using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cashing
{
    public sealed class CachedComponentsContainer
    {
        private readonly Dictionary<Type, Component> _cachedComponents = new();
        // TODO fix to be more reasonable
        private readonly Dictionary<Type, bool> _hasComponent = new();
        
        private GameObject _ownerObject;

        public void Initialize(GameObject ownerObject)
        {
            _ownerObject = ownerObject;
        }
        
        #region Typed
        public bool HasComponent(Type componentType)
        {
            if (_hasComponent.TryGetValue(componentType, out bool result)) return result;
            
            if (TryGetComponentFromOwner(componentType, out Component component))
            {
                _cachedComponents.Add(componentType, component);
                _hasComponent.Add(componentType, true);
                return true;
            }
            
            _hasComponent.Add(componentType, false);
            return false;
        }
        public Component Get(Type componentType)
        {
            if (!_cachedComponents.TryGetValue(componentType, out Component component)) 
            {
                component = CashComponent(componentType);
            }

            return component;
        }
        
        private Component CashComponent(Type componentType)
        {
            Component component = GetComponentFromOwner(componentType);
            
            _cachedComponents.Add(componentType, component);
            _hasComponent.Add(componentType, true);

            return component;
        }
        private Component GetComponentFromOwner(Type componentType)
        {
            if (TryGetComponentFromOwner(componentType, out Component component))
            {
                return component;
            }
            else
            {
                throw new MissingComponentException($"Couldn't find {componentType} component on object & children");
            }
        }
        private bool TryGetComponentFromOwner(Type componentType, out Component component)
        {
            component = _ownerObject.GetComponent(componentType);
            if (component) return true;
            
            component = _ownerObject.GetComponentInChildren(componentType);
            if (component) return true;

            return false;
        }
        
        #endregion
        
        #region Generic
        public bool Has<T>() where T : Component
        {
            return HasComponent(typeof(T));
        }
        public T Get<T>() where T : Component 
        {
            return (T)Get(typeof(T));
        }
        private Component CashComponent<T>() where T: Component
        {
            return CashComponent(typeof(T));
        }
        private T GetComponentFromOwner<T>() where T : Component
        {
            return (T)GetComponentFromOwner(typeof(T));
        }
        #endregion
    }
}