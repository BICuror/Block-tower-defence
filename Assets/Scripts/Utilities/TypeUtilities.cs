using System.Collections.Generic;
using System.Reflection;
using System;

public static class TypeUtilities
{
    public static List<Type> GetThisAndAllDerivedTypes(this Type type)
    {
        List<Type> types = new();
        
        Type currentType = type;

        while (currentType != null)
        {
            types.Add(currentType);
            
            currentType = currentType.BaseType;   
        }
            
        return types;
    }
    
    public static List<Type> GetThisAndAllDerivedTypesUntil(this Type type, Type stopType)
    {
        List<Type> types = new();
        
        Type currentType = type;

        while (currentType != null && currentType != stopType)
        {
            types.Add(currentType);
            
            currentType = currentType.BaseType;
        }
            
        return types;
    }

    public static List<FieldInfo> GetAllFieldsFromThisAndDerivedTypes(this Type type)
    {
        List<FieldInfo> fieldInfos = new();
        
        foreach (Type derivedType in type.GetThisAndAllDerivedTypes())
        {
            foreach (FieldInfo fieldInfo in derivedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                fieldInfos.Add(fieldInfo);
            }
        }
        
        return fieldInfos;
    }
}
