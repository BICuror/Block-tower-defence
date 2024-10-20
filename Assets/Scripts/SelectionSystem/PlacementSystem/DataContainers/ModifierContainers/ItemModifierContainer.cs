using UnityEngine;
using System.Collections.Generic;

public class ItemModifierContainer<T> : ScriptableObject where T: ItemModifierData
{
    [SerializeField] private List<T> _positiveModifiers;
    [SerializeField] private List<T> _negativeModifiers;

    public List<T> PositiveModifiers => _positiveModifiers;
    public List<T> NegativeModifiers => _negativeModifiers;
}