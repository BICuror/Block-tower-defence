using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

namespace Cashing
{
    public abstract class EntityComponentCacher : MonoBehaviour
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        private readonly string[] IGNORED_NAMESPACES = new string[]
        { 
            "UnityEngine", 
            "UnityEditor"
        };
        
        [SerializeField] private StatInitializer[] _statInitializers;
        
        private readonly StatContainer _statContainer = new();
        private readonly CachedComponentsContainer _cachedComponentsContainer = new();
        
        public StatContainer StatContainer => _statContainer;
        public CachedComponentsContainer ComponentsContainer => _cachedComponentsContainer;

        protected void Awake()
        {
            _cachedComponentsContainer.Initialize(gameObject);
            InitializeStatContainer();
            
            InjectAll();
        }

        #region Cache

        private void InjectAll() => InjectCachedToObjectAndChildren(gameObject);
        
        public void InjectCachedToObjectAndChildren(GameObject injectReciver)
        {
            Component[] components = injectReciver.GetComponentsInChildren<Component>().ToArray();
            
            for (int i = 0; i < components.Length; i++) 
            {
                InjectCached(components[i]);
            }
        }

        public void InjectCached(object injectReciverObject)
        {
            Type componentType = injectReciverObject.GetType();
            
            if (IsIgnoredComponent(componentType)) return;
            
            if (IsInjectable(componentType)) CacheFields(GetFieldsToCache(componentType), injectReciverObject);
            
            List<Type> subTypes = GetInjectableSubTypes(injectReciverObject);

            subTypes.ForEach(subType =>
            {
                CacheFields(GetFieldsToCache(subType), injectReciverObject);
            });
        }

        private bool IsIgnoredComponent(Type type)
        {
            string namespaceNames = type.Namespace;

            if (namespaceNames == null) return false;

            for (int i = 0; i < IGNORED_NAMESPACES.Length; i++)
            {
                if (namespaceNames.Contains(IGNORED_NAMESPACES[i])) return true;
            }

            return false;
        }
        
        private List<Type> GetInjectableSubTypes(object injectReciverObject)
        {
            List<Type> subTypes = new();
            
            Type baseType = injectReciverObject.GetType().BaseType;
            
            while (baseType != null && baseType != typeof(Component))
            {
                if (IsInjectable(baseType))
                {
                    subTypes.Add(baseType);
                }
                    
                baseType = baseType.BaseType;
            }
            
            return subTypes;
        }

        private bool IsInjectable(Type type)
        {
            IEnumerable<FieldInfo> fields = type.GetFields(BINDING_FLAGS);
            
            return fields.Any(member => Attribute.IsDefined(member, typeof(CachedAttribute)));
        }
        
        private FieldInfo[] GetFieldsToCache(Type type) 
        {
            FieldInfo[] fields = type.GetFields(BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(CachedAttribute))).ToArray();
            
            return fields;
        }
        
        private void CacheFields(FieldInfo[] fields, object injectReciverObject) 
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Type type = fields[i].FieldType;
                try
                {
                    fields[i].SetValue(injectReciverObject, Resolve(type));
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to resolve component {type.Name} for {injectReciverObject.GetType().Name}");
                }
            }
        }

        private object Resolve(Type type)
        {
            if (type.IsSubclassOf(typeof(Stat)))
            {
                return _statContainer.Get(type);
            }
            
            return _cachedComponentsContainer.Get(type);
        }

        #endregion

        #region StatContainer
        
        private void InitializeStatContainer()
        {
            _statContainer.AddStats(_statInitializers);
            _cachedComponentsContainer.Add<StatContainer>(_statContainer);
        }
        
        private void OnValidate()
        {
            for (int i = 0; i < _statInitializers.Length; i++)
            {
                _statInitializers[i].StructName = _statInitializers[i].StatData.GetStatType().ToString();

                Type statType = _statInitializers[i].StatData.GetStatType();
            
                if (_statContainer.Has(statType))
                {
                    _statContainer.Get(statType).SetDefault(_statInitializers[i].DefaultValue);
                }
            }
        }
        
        #endregion
    }
}