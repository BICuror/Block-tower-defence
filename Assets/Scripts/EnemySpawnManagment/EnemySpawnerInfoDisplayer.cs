using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawnerInfoDisplayer : MonoBehaviour
{
    [SerializeField] private SpawnInfoObject _spawnInfoObjectPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private float _distanceBetweenInfoObjects;

    private List<SpawnInfoObject> _spawnInfoObjects = new();
    private bool _showInfoObjects;

    public void SetSpawnInfo(List<EnemyData> enemiesToSpawn)
    {
        if (_showInfoObjects)
        {
            HideSpawnInfo();
            _showInfoObjects = true;
        }
        
        _spawnInfoObjects.Clear();
        
        Dictionary<EnemyData, int> datas = new Dictionary<EnemyData, int>();
        List<EnemyData> countedDatas = new List<EnemyData>();

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            if (datas.ContainsKey(enemiesToSpawn[i]) == false)
            {
                datas.Add(enemiesToSpawn[i], 1);
                countedDatas.Add(enemiesToSpawn[i]);
            }
            else
            {
                datas[enemiesToSpawn[i]] += 1;
            }
        }

        float halfDistance = (_distanceBetweenInfoObjects * (datas.Keys.Count - 1)) / 2;

        for (int i = 0; i < datas.Keys.Count; i++)
        {
            SpawnInfoObject infoObject = Instantiate(_spawnInfoObjectPrefab, _parent.transform.position, _parent.rotation, _parent);
            infoObject.transform.localPosition = new Vector3(_distanceBetweenInfoObjects * i - halfDistance, 0f, 0f);
            infoObject.transform.Rotate(90f, 0, 180f);
            infoObject.transform.localScale = Vector3.zero;
            infoObject.gameObject.SetActive(false);
            
            _spawnInfoObjects.Add(infoObject);

            infoObject.SetEnemiyData(countedDatas[i]);
            infoObject.SetAmount(datas[countedDatas[i]]); 
        }
        
        if (_showInfoObjects) ShowSpawnInfo();
    }

    public void HideSpawnInfo()
    {
        _showInfoObjects = false;

        for (int i = 0; i < _spawnInfoObjects.Count; i++)
        {
            _spawnInfoObjects[i].Disappear();
        }
    }

    public void ShowSpawnInfo()
    {
        _showInfoObjects = true;
        
        for (int i = 0; i < _spawnInfoObjects.Count; i++)
        {
            _spawnInfoObjects[i].Appear();
        }
    }
}
