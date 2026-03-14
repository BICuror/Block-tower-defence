using WorldGeneration;
using Zenject;

public sealed class Chest : OptionalTask
{
    [Inject] private SpawnerRotator _spawnerRotator;
    
    private void Start()
    {
        base.Start();
        _spawnerRotator.RotateSpawner(transform);
    }

    protected override bool IsCompleted() => true;
}