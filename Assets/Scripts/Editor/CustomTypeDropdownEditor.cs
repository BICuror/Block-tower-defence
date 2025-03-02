using System.Collections.Generic;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

public abstract class CustomTypeDropdownEditor<T> : Editor
{
    private void OnEnable()
    {
        Type T = typeof(T);
        
        List<string> _objectTypeName = new(); 
        
        foreach (Type type in System.Reflection.Assembly.GetAssembly(T).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(T)))
        { 
            _objectTypeName.Add(type.ToString());
        }
        
        _objectTypeName.Sort();

        ApplyDropdownItems(_objectTypeName);
        
        serializedObject.ApplyModifiedProperties();
        
        serializedObject.Update();
    }

    protected abstract void ApplyDropdownItems(List<string> items);
}
#endif