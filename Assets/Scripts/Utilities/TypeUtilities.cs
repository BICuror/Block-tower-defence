using System.Collections.Generic;
using System.Reflection;
using System;

public static class TypeUtilities
{
    public static List<Type> GetAllDerivedTypes(this Type type)
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

    public static List<FieldInfo> GetAllFieldsFromThisAndDerivedTypes(this Type type)
    {
        List<FieldInfo> fieldInfos = new();
        
        foreach (Type derivedType in type.GetAllDerivedTypes())
        {
            foreach (FieldInfo fieldInfo in derivedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                fieldInfos.Add(fieldInfo);
            }
        }
        
        return fieldInfos;
    }
}
