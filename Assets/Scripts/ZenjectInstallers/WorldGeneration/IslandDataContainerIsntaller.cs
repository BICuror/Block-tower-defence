using UnityEngine;
using Zenject;
using WorldGeneration;

[CreateAssetMenu(menuName = "Installers/IslandDataContainerIsntaller")]

public sealed class IslandDataContainerIsntaller : ScriptableObjectInstaller<IslandDataContainerIsntaller> 
{
    [SerializeField] private IslandDataContainer _islandDataContainer;

    public override void InstallBindings()
    {
        Container.Bind<IslandDataContainer>().FromInstance(_islandDataContainer).AsSingle().NonLazy();
    }
}
