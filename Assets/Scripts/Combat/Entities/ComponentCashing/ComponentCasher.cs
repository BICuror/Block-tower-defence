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

        private void InjectCached(Component component)
        {
            Type componentType = component.GetType();
            
            if (IsIgnoredComponent(componentType)) return;
            
            if (IsInjectable(componentType))
                CacheFields(GetFieldsToCache(componentType), component);
            
            List<Type> subTypes = GetInjectableSubTypes(component);

            subTypes.ForEach(subType =>
            {
                CacheFields(GetFieldsToCache(subType), component);
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
        
        private List<Type> GetInjectableSubTypes(Component component)
        {
            List<Type> subTypes = new();
            
            Type baseType = component.GetType().BaseType();
            
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
        
        private void CacheFields(FieldInfo[] fields, Component component) 
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Type type = fields[i].FieldType;
                try
                {
                    fields[i].SetValue(component, Resolve(type));
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to resolve component {type.Name} for {component.GetType().Name}");
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