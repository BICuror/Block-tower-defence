using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawnerInfoDisplayer : MonoBehaviour
{
    [SerializeField] private SpawnInfoObject _spawnInfoObjectPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private float _distanceBetweenInfoObjects;

    private List<SpawnInfoObject> _spawnInfoObjects;

    public void DisplaySpawnInfo(List<EnemyData> enemiesToSpawn)
    {   
        if (enemiesToSpawn.Count < 10)
        {
            float halfDistance = (_distanceBetweenInfoObjects * (enemiesToSpawn.Count - 1)) / 2;

            _spawnInfoObjects = new List<SpawnInfoObject>();
            
            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                SpawnInfoObject spo = Instantiate(_spawnInfoObjectPrefab, _parent.transform.position, _parent.rotation, _parent);
                spo.transform.localPosition = new Vector3(_distanceBetweenInfoObjects * i - halfDistance, 0f, 0f);
                spo.transform.Rotate(90f, 0, 180f);

                _spawnInfoObjects.Add(spo);

                spo.SetEnemiyData(enemiesToSpawn[i]);

                spo.Appear();
            }
        }
        else
        {
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

            _spawnInfoObjects = new List<SpawnInfoObject>();

            for (int i = 0; i < datas.Keys.Count; i++)
            {
                SpawnInfoObject spo = Instantiate(_spawnInfoObjectPrefab, _parent.transform.position, _parent.rotation, _parent);
                spo.transform.localPosition = new Vector3(_distanceBetweenInfoObjects * i - halfDistance, 0f, 0f);
                spo.transform.Rotate(90f, 0, 180f);

                _spawnInfoObjects.Add(spo);

                spo.SetEnemiyData(countedDatas[i]);
                spo.SetAmount(datas[countedDatas[i]]); 
            }
        }
    }

    public void HideSpawnInfo()
    {
        for (int i = 0; i < _spawnInfoObjects.Count; i++)
        {
            _spawnInfoObjects[i].Disappear();
        }

        _spawnInfoObjects = new List<SpawnInfoObject>();
    }
}
