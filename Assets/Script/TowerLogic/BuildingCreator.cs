using System.Collections.Generic;
using UnityEngine;

public sealed class BuildingCreator : MonoBehaviour
{
    [SerializeField] private LayerMask _terrainLayer;    
    [SerializeField] private LayerMask _acseptableLayer;   

    [SerializeField] private int _minSpawnRadius;
    [SerializeField] private int _maxSpawnRadius;

    [SerializeField] private GameObject _offerContainer;

    [Range(20f, 85f)] [SerializeField] private float _launchAngle;

    public void CreateBuildings(BuildingOffer buildingOffer)
    {
        for (int i = 0; i < buildingOffer.GetBuildingAmount(); i++)
        {
            CreateBuilding(buildingOffer.GetBuildingPrefab());
        }
    }

    public void CreateBuilding(GameObject buildingToSpawn)
    {
        GameObject offerContainer = Instantiate(_offerContainer, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);

        GameObject createdTower = Instantiate(buildingToSpawn, offerContainer.transform.position, offerContainer.transform.rotation, offerContainer.transform);    

        offerContainer.GetComponent<Rigidbody>().velocity = CalculateVelocity(GetRandomSpawnPosition(_maxSpawnRadius));
    }

    private Vector3 GetRandomSpawnPosition(float raduis)
    {
        List<Vector3> possiblePositions = new List<Vector3>();

        for (int x = (int)(transform.position.x - raduis); x < transform.position.x + raduis + 1; x++)
        {
            for (int z = (int)(transform.position.z - raduis); z < transform.position.z + raduis + 1; z++)
            {
                if ((x >= (int)(transform.position.x - _minSpawnRadius) && x <= (int)(transform.position.x) + _minSpawnRadius) || z >= (int)(transform.position.z - _minSpawnRadius) && z <= (int)(transform.position.z) + _minSpawnRadius)
                {
                    Ray heightRay = new Ray(new Vector3(x, 40f, z), new Vector3(0f, -1f, 0f));
                
                    if (Physics.Raycast(heightRay, out RaycastHit rayInfo, Mathf.Infinity, _acseptableLayer))
                    {
                        if (rayInfo.collider.gameObject.CompareTag("Terrain")) possiblePositions.Add(new Vector3(x, rayInfo.point.y + 0.5f, z));
                    }
                }
            }
        }

        if (possiblePositions.Count > 0) return possiblePositions[Random.Range(0, possiblePositions.Count)];
        else return GetRandomSpawnPosition(raduis + 1);
    }

    private Vector3 CalculateVelocity(Vector3 destination)
    {     
        Vector3 direction = destination - transform.position - new Vector3(0f, 1f, 0f); // get Target Direction
        float height = direction.y; // get height difference
        direction.y = 0; // retain only the horizontal difference
        float distance = direction.magnitude; // get horizontal direction
        float radians = _launchAngle * Mathf.Deg2Rad; // Convert angle to radians
        direction.y = distance * Mathf.Tan(radians); // set dir to the elevation angle.
        distance += height / Mathf.Tan(radians); // Correction for small height differences

        // Calculate the velocity magnitude
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * radians));

        return velocity * direction.normalized;
    }
}
