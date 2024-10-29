using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CombatEntity : MonoBehaviour
{
    private Dictionary<Type, MonoBehaviour> _conmponentDictionary;

    public void AddComponent<T>(T component) where T : MonoBehaviour
    {
        _conmponentDictionary.Add(T, component);
    }

    public void RemoveComponent<T>(T component) where T: MonoBehaviour
    {
        _conmponentDictionary.Remove(T);
    }
}