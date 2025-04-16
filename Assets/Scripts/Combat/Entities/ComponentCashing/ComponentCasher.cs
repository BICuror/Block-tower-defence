using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModestTree;
using UnityEngine;

namespace Cashing
{
    [RequireComponent(typeof(StatContainer))]
    
    public abstract class EntityComponentCacher : MonoBehaviour
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        private readonly string[] IGNORED_NAMESPACES = new string[]
        { 
            "UnityEngine", 
            "UnityEditor"
        };
        private StatContainer StatContainer;
        
        protected readonly CachedComponentsContainer CachedComponentsContainer = new();

        protected void Awake()
        {
            StatContainer = GetComponent<StatContainer>();
            
            CachedComponentsContainer.Initialize(gameObject); 
            StatContainer.Initialize();
            
            InjectAll();
        }

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
            
            if (IsInjectable(componentType))
                CacheFields(GetFieldsToCache(componentType), injectReciverObject);
            
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
            
            Type baseType = injectReciverObject.GetType().BaseType();
            
            while (baseType != null && baseType != typeof(Component))
            {
                if (IsInjectable(baseType))
                {
                    subTypes.Add(baseType);
                }
                    
                baseType = baseType.BaseType();
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
                return StatContainer.Get(type);
            }
            
            return CachedComponentsContainer.Get(type);
        }
    }
}