using System.Collections.Generic;
using System;

public sealed class ListDictionary<T> 
{
    private Dictionary<Type, List<T>> _dictionary = new();
    private List<T> _allItems = new();
    
    public delegate List<T> ListDictionarySorter(List<T> initialList);
    private ListDictionarySorter _sorter;
    
    public ListDictionary(ListDictionarySorter sorter)
    {
        _sorter = sorter;
    }
    
    public void Add(Type type, T value)
    {
        if (_dictionary.ContainsKey(type))
        {
            _dictionary[type].Add(value);
        }
        else
        {
            _dictionary.Add(type, new List<T>() { value });
        }
        
        _allItems.Add(value);
        _allItems = _sorter.Invoke(_allItems);
    }

    public void Remove(Type type)
    {
        T value = _dictionary[type][^1];
        _dictionary[type].Remove(value);
        
        _allItems.Remove(value);
        _allItems = _sorter.Invoke(_allItems);
    }

    public bool Contains(Type type)
    {
        if (_dictionary.TryGetValue(type, out List<T> list))
        {
            return list.Count > 0;
        }

        return false;
    }
    
    public List<T> GetAllItems() => _allItems;
}