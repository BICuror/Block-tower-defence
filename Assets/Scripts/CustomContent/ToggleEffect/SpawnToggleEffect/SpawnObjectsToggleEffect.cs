using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using WorldGeneration;
using DG.Tweening;
using UnityEngine;
using Zenject;
using System;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SpawnObjectsToggleEffect : GlobalEffect
{
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private RoadMapHolder _roadMapHolder;
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isInitialized;
    private int _randomSeed;
    private System.Random _random;
    
    private GameObject _objectPrefab;
    private float _maxObjectScale;
    private float _minObjectScale;
    private int _objectAmount;
    
    private List<GameObject> _instantiatedObjects = new();
    
    private void TryInitialize()
    {
        if (_isInitialized) return;

        _isInitialized = true;

        _objectPrefab = Args.GetArgument<GameObject>("ObjectPrefab");
        _maxObjectScale = Args.GetArgument<float>("MaxObjectScale");
        _minObjectScale = Args.GetArgument<float>("MinObjectScale");
        _objectAmount = Args.GetArgument<int>("ObjectAmount");
    }
    
    public override void Enable()
    {
        CancelPreviousTask();
        
        TryInitialize();
        
        _random = new System.Random(_enemySpawnGroupCompiler.CurrentWaveSeed);
        
        SpawnObjects().Forget();
    }

    private async UniTask SpawnObjects()
    {
        for (int i = 0; i < _objectAmount;)
        {
            if (TrySpawnObject())
            {
                try
                {
                    await UniTask.WaitForFixedUpdate(_cancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    e.LogAsync();
                    return;
                }

                i++;
            }
        }
    }

    private bool TrySpawnObject()
    {
        int x = _random.Next(0, _islandDataContainer.Data.IslandSize);
        int z = _random.Next(0, _islandDataContainer.Data.IslandSize);

        Vector2Int spawnPosition = new Vector2Int(x, z);
        
        if (!IsValidSpawnPosition(spawnPosition)) return false;
        
        int y = _islandHeightMapHolder.Map[x, z];

        GameObject instantiatedObject = Object.Instantiate(_objectPrefab, new Vector3(x, y + 1.5f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        
        _instantiatedObjects.Add(instantiatedObject);
        
        AnimateObject(instantiatedObject).Forget();

        return true;
    }

    private async UniTask AnimateObject(GameObject instantiatedObject)
    {
        float scale = Random.Range(_minObjectScale, _maxObjectScale);

        instantiatedObject.transform.localScale = Vector3.zero;
        instantiatedObject.transform.DOMoveY(instantiatedObject.transform.position.y - 1f, 0.5f);
        await instantiatedObject.transform.DOScale(new Vector3(scale, scale, scale), 0.5f).AsyncWaitForCompletion();
    }

    protected virtual bool IsValidSpawnPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < _islandDataContainer.Data.IslandSize && position.y >= 0 && position.y < _islandDataContainer.Data.IslandSize && 
            _islandHeightMapHolder.Map[position.x, position.y] > 0 && !_roadMapHolder.Map[position.x, position.y] && 
            !TileMap.HasTile(position, LayerSettingType.SolidObjects);
    }

    private void CancelPreviousTask()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }
    
    public override void Disable()
    {
        CancelPreviousTask();
        
        DestroyObjects().Forget();

        _instantiatedObjects.Clear();
    }

    private async UniTask DestroyObjects()
    {
        List<GameObject> instantiatedObjects = new(_instantiatedObjects);
        
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            try
            {
                await UniTask.WaitForFixedUpdate(_cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                return;
            }
            
            DestroyObject(instantiatedObjects[i]).Forget();
        }
    }

    private async UniTask DestroyObject(GameObject objectToDestroy)
    {
        objectToDestroy.transform.DOKill();
        
        await objectToDestroy.transform.DOScale(Vector3.zero, 0.5f).AsyncWaitForCompletion();
        
        Object.Destroy(objectToDestroy);
    }
}