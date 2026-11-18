using Configs;
using Cysharp.Threading.Tasks;
using ScriptableLogic.ArgumentItem;
using ScriptableLogic.Commands;
using UnityEngine;
using Zenject;

public sealed class CreateBombs : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= Entity.ComponentsContainer.Get<BombCreatorTower>().CreateBomb;
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Entity.ComponentsContainer.Get<BombCreatorTower>().CreateBomb;
    }
}

public class CreateBombCommand : CreateDraggableObjectAroundCommand
{
    protected virtual void OnDraggableCreated(DraggableObject draggableObject)
    {
        Bomb bomb = draggableObject.GetComponent<Bomb>();
        bomb.EnableExplosion();
    }
}

public class CreateDraggableObjectAroundCommand : Command, ICommand, ICommand<Vector3>
{
    [Inject] private DraggableCreator _draggableCreator;
    
    [Argument("DraggablePrefab")] private GameObject _draggablePrefab;
    [Argument("LauncherPrefab")] private Launcher _launcherPrefab;
    [Argument("DontDestroyOnDisable")] private bool _dontDestroyOnDisable;
    [Argument("SpawnRadius")] private int _spawnRadius;

    public UniTask Execute() => Execute(Args.Owner.transform.position);

    public async UniTask Execute(Vector3 args)
    {
        await CreateDraggableObject(args);
    }

    private async UniTask CreateDraggableObject(Vector3 spawnPosition)
    {
        DraggableObject draggableObject = await _draggableCreator.CreateDraggableOnRandomPosition(_draggablePrefab.GetComponent<DraggableObject>(), spawnPosition, _spawnRadius, _launcherPrefab);

        TryAwakeDisablementToDestroyObject(draggableObject.gameObject).Forget();

        OnDraggableCreated(draggableObject);
    }
    
    protected virtual void OnDraggableCreated(DraggableObject draggableObject) {}

    private async UniTask TryAwakeDisablementToDestroyObject(GameObject gameObject)
    {
        if (_dontDestroyOnDisable) return;
        
        await UniTask.WaitWhile(() => gameObject.activeSelf);
        await UniTask.WaitForFixedUpdate();
        
        if (gameObject) Object.Destroy(gameObject);
    }
}