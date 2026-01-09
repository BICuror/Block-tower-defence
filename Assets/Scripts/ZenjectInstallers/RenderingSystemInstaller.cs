using UnityEngine;
using Zenject;

public sealed class RenderingSystemInstaller : MonoInstaller
{
    [SerializeField] private PostProcessingController _postProcessingController;
    
    public override void InstallBindings()
    {
        Container.Bind<PostProcessingController>().FromInstance(_postProcessingController).AsSingle().NonLazy();
    }
}