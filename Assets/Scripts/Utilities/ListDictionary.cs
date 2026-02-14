using System.Collections.Generic;
using System.Linq;

public sealed class ListDictionary<TKey, TValue> 
{
    private Dictionary<TKey, List<TValue>> _dictionary = new();
    private List<TValue> _allItems = new();
    
    public delegate List<TValue> ListDictionarySorter(List<TValue> initialList);
    private ListDictionarySorter _sorter;
    
    public ListDictionary() {}
    
    public ListDictionary(ListDictionarySorter sorter)
    {
        _sorter = sorter;
    }
    
    public void Add(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
        {
            _dictionary[key].Add(value);
        }
        else
        {
            _dictionary.Add(key, new List<TValue>() { value });
        }
        
        _allItems.Add(value);
        
        TrySort();
    }

    public void Remove(TKey key, TValue value)
    {
        _dictionary[key].Remove(value);
        
        _allItems.Remove(value);
        
        if (_dictionary[key].Count == 0) _dictionary.Remove(key);
        
        TrySort();
    }
    
    public TValue Remove(TKey key)
    {
        TValue value = _dictionary[key][^1];
        _dictionary[key].Remove(value);
        
        _allItems.Remove(value);
        
        if (_dictionary[key].Count == 0) _dictionary.Remove(key);

        TrySort();

        return value;
    }

    public List<TValue> Get(TKey key)
    {
        return _dictionary[key];
    }
    
    public bool Contains(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }
    
    public List<TValue> GetAllItems() => _allItems;
    public List<TKey> GetAllKeys() => _dictionary.Keys.ToList();

    private void TrySort()
    {
        if (_sorter != null) _allItems = _sorter.Invoke(_allItems);
    }
}