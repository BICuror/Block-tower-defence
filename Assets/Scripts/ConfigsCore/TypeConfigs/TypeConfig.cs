using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

namespace Configs
{
    public abstract class TypeConfig : ScriptableObject
    {
        [Header("BaseData")]
        [SerializeField] private string _typeName;
        [SerializeField, ReadOnly] private string _genericType;
        
        public Type GetContainedType() => Type.GetType(_typeName);

        public bool TryGetGenericType(out List<Type> genericArgumentTypes)
        {
            genericArgumentTypes = new();
            
            Type type = GetContainedType();

            if (type == null) return false;

            if (TryFindAllGenericTypes(out List<Type> genericTypes, type))
            {
                genericArgumentTypes = GetAllGenericArgumentTypes(genericTypes);
                
                return true;
            }
            
            return false;
        }

        private bool TryFindAllGenericTypes(out List<Type> genericTypes, Type derivedType)
        {
            genericTypes = new();
            Type currentType = derivedType;

            while (currentType != null)
            {
                if (currentType.IsGenericType) genericTypes.Add(currentType);
                else
                {
                    foreach (Type interfaceType in currentType.GetInterfaces())
                    {
                        if (interfaceType.IsGenericType) genericTypes.Add(currentType);
                        break;
                    }
                }
                    
                currentType = currentType.BaseType;   
            }
            
            return genericTypes.Count > 0;
        }

        private List<Type> GetAllGenericArgumentTypes(List<Type> genericTypes)
        {
            List<Type> genericArgumentTypes = new();

            foreach (Type genericType in genericTypes)
            {
                if (genericType.IsGenericType && genericType.GetGenericArguments().Length > 0)
                {
                    genericArgumentTypes.AddRange(genericType.GetGenericArguments());
                }
                foreach (Type interfaceType in genericType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericArguments().Length > 0) genericArgumentTypes.AddRange(interfaceType.GetGenericArguments());
                }
            }
            
            return genericArgumentTypes;
        }
        
        private void OnValidate()
        {
            if (GetContainedType() == null)
            {
                _genericType = "Invalid type";
                
                return;
            }

            if (TryGetGenericType(out List<Type> genericTypes))
            {
                _genericType = String.Empty;
                
                foreach (Type genericType in genericTypes)
                {
                    if (!string.IsNullOrEmpty(_genericType)) _genericType += ", ";

                    _genericType += genericType.Name;
                }
            }
            else
            {
                _genericType = "Non generic";
            }
        }
    }
}