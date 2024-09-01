using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Zenject;

public sealed class DraggableCreator : MonoBehaviour 
{
    [Header("SpawnPositionSettings")]
    [SerializeField] private GameObject _blankDraggable;

    [SerializeField] private LayerSetting _terrainLayerSettings;    
    [SerializeField] private LayerSetting solidObjectsLayerSettings;   

    [Range(1f, 5f)] [SerializeField] private int _spawnRadius;

    [Space] [Header("LaunchSettings")]
    [SerializeField] private float _launchDuration;

    [SerializeField] private float _launchMaxHeight;
    
    [SerializeField] private Launcher _defaultLauncherPrefab;
    
    private YieldInstruction _yieldInstruction = new WaitForFixedUpdate(); 

    [Inject] private DiContainer _diContainer;
    
    public void CreateDraggableOnPosition(DraggableObject draggablePrefab, Vector3 centerPosition, Vector3 finalPosition)
    {       
        Launcher launcher = CreateLauncher(_defaultLauncherPrefab, centerPosition, finalPosition);

        launcher.SetDraggablePrefab(draggablePrefab);

        CreateBlankDraggable(finalPosition);
    }

    public Launcher CreateDraggableOnRandomPosition(DraggableObject draggablePrefab, Vector3 centerPosition, [Optional]int radius, [Optional]LayerSetting terrainLayer, [Optional]Launcher launcherPrefab)
    {
        if (radius == 0) radius = _spawnRadius;
        if (terrainLayer == null) terrainLayer = _terrainLayerSettings; 
        if (launcherPrefab == null) launcherPrefab = _defaultLauncherPrefab;

        Vector3 finalPosition = GetRandomSpawnPosition(centerPosition, radius, terrainLayer);
            
        Launcher launcher = CreateLauncher(launcherPrefab, centerPosition, finalPosition);

        launcher.SetDraggablePrefab(draggablePrefab);

        CreateBlankDraggable(finalPosition);

        return launcher;
    }

    private void CreateBlankDraggable(Vector3 spawnPosition)
    {
        GameObject blankDraggable = Instantiate(_blankDraggable, spawnPosition, Quaternion.identity);

        Destroy(blankDraggable, _launchDuration);
    }

    #region LauncherCreation
    private Launcher CreateLauncher(Launcher launcherPrefab, Vector3 centerPosition, Vector3 finalPosition)
    {
        Launcher launcher = _diContainer.InstantiatePrefab(launcherPrefab.gameObject, centerPosition, Quaternion.identity, null).GetComponent<Launcher>();

        StartCoroutine(LaunchLauncher(launcher, finalPosition));

        return launcher;
    }

    private IEnumerator LaunchLauncher(Launcher launcher, Vector3 finalPosition)
    {
        Vector3 startPosition = launcher.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < _launchDuration)
        {
            yield return _yieldInstruction;

            elapsedTime += Time.deltaTime;

            float currentProgress = elapsedTime / _launchDuration;

            Vector3 evaluetedPosition = Vector3.Lerp(startPosition, finalPosition, currentProgress);

            evaluetedPosition.y = evaluetedPosition.y + (Mathf.Sin(currentProgress * 180f * Mathf.Deg2Rad)) * _launchMaxHeight; 

            launcher.transform.position = evaluetedPosition;
        }

        launcher.Land(finalPosition);
    }
    #endregion

    #region SpawnPositionPicking
    private Vector3 GetRandomSpawnPosition(Vector3 centerPosition, int radius, LayerSetting terrainLayer)
    {
        List<Vector3> allPossiblePositions = new List<Vector3>();

        int currentRadius = radius;

        Vector2Int roundedCenterPosition = new Vector2Int(Mathf.RoundToInt(centerPosition.x), Mathf.RoundToInt(centerPosition.z));

        while(allPossiblePositions.Count == 0)
        {
            currentRadius++;

            allPossiblePositions.AddRange(GetPossiblePositionsInRadius(roundedCenterPosition, currentRadius, terrainLayer));
        }
        
        return allPossiblePositions[Random.Range(0, allPossiblePositions.Count)];
    }

    private List<Vector3> GetPossiblePositionsInRadius(Vector2Int roundedCenterPosition, int radius, LayerSetting terrainLayer)
    {
        List<Vector3> possiblePositions = new List<Vector3>();

        int xMinPosition = roundedCenterPosition.x - radius;
        int xMaxPosition = roundedCenterPosition.x + radius;

        int zMinPosition = roundedCenterPosition.y - radius;
        int zMaxPosition = roundedCenterPosition.y + radius;

        for (int x = xMinPosition; x <= xMaxPosition; x++)
        {
            for (int z = zMinPosition; z <= zMaxPosition; z++)
            {
                if (x == xMinPosition || z == zMinPosition || x == xMaxPosition || z == zMaxPosition )
                {
                    Ray heightRay = new Ray(new Vector3(x, 10000f, z), Vector3.down);
                    
                    if (Physics.Raycast(heightRay, out RaycastHit rayInfo, Mathf.Infinity, terrainLayer.GetLayerMask()))
                    {
                        RaycastHit[] hits = Physics.RaycastAll(new Vector3(x, 10000f, z), Vector3.down, Mathf.Infinity, solidObjectsLayerSettings.GetLayerMask());

                        if (hits.Length == 0)
                        {
                            possiblePositions.Add(new Vector3(x, rayInfo.point.y + 0.5f, z));
                        }
                    }                   
                }
            }
        }
        return possiblePositions;
    }
    #endregion
}
