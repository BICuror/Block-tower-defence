using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Townhall : MonoBehaviour
{
    [SerializeField] private GameObject _chargePrefab;

    [SerializeField] private GameObject _timeTower;

    [Range(1, 10)] [SerializeField] private int _chargesAmount;

    [SerializeField] private BuildingCreator _buildingCreator;

    private int _leftCharges; 

    private void Start()
    {     
        FindObjectOfType<CameraRotationController>().SetTarget(transform);

        GetComponent<IDraggable>().Place();

        //StartWave();
    }

    public void StartWave()
    {
        _leftCharges = _chargesAmount;

        CreateCharges();
    }

    private void CreateCharges()
    {_buildingCreator.CreateBuilding(_timeTower);
        for (int i = 0; i < _chargesAmount; i++)
        {
            _buildingCreator.CreateBuilding(_chargePrefab);

            
        }
    } 

    public void TryStartNextWave()  
    {
        _leftCharges --;

        if (_leftCharges == 0) StartWave();
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene(0);
    }
}

