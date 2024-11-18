using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cashing
{
    public sealed class CachedComponentsContainer
    {
        private readonly Dictionary<Type, Component> _cachedComponents = new();
        private GameObject _ownerObject;

        public void Initialize(GameObject ownerObject)
        {
            _ownerObject = ownerObject;
        }
        
        #region Typed
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

            return component;
        }
        private Component GetComponentFromOwner(Type componentType)
        {
            Component component = _ownerObject.GetComponent(componentType);
            if (component) return component;
            
            component = _ownerObject.GetComponentInChildren(componentType);
            if (component) return component;
            
            throw new MissingComponentException($"Couldn't find {componentType} component on object & children");
        }
        #endregion
        
        #region Generic

        public bool Has<T>() where T : Component
        {
            return _cachedComponents.ContainsKey(typeof(T));
        }
        public T Get<T>() where T : Component 
        {
            Component component;
            
            Type componentType = typeof(T);

            if (!_cachedComponents.TryGetValue(componentType, out component)) 
            {
                component = CashComponent<T>();
            }
            
            return (T)component;
        }
        private Component CashComponent<T>() where T: Component
        {
            Component component = GetComponentFromOwner<T>();
            
            _cachedComponents.Add(typeof(T), component);

            return component;
        }
        private T GetComponentFromOwner<T>() where T : Component
        {
            if (_ownerObject.TryGetComponent(out T foundComponent)) return foundComponent;
            
            T component = _ownerObject.GetComponentInChildren<T>();
            if (!component) return component;
            
            throw new MissingComponentException($"Couldn't find {typeof(T).ToString()} component on object & children");
        }
        #endregion
    }
}