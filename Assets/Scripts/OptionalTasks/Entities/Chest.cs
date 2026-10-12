using WorldGeneration;
using Zenject;

namespace Combat.OptionalTasks
{
    public sealed class Chest : OptionalTask
    {
        [Inject] private SpawnerRotator _spawnerRotator;

        private void Start()
        {
            _spawnerRotator.RotateSpawner(transform);
        }

        protected override bool IsCompleted() => true;
    }
}