using System.Collections.Generic;
using UnityEngine;

public class SelectionOptionDataContainer<T> : ScriptableObject where T: SelectionOptionData 
{
    [SerializeField] private List<T> _optionDatas;
    
    public List<T> GetDatas(int amount)
    {
        List<T> datas = new List<T>(_optionDatas);
        List<T> result = new();

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, datas.Count);

            result.Add(datas[randomIndex]);
            datas.RemoveAt(randomIndex);
        }

        return result;
    }
}