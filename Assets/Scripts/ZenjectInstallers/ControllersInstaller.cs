using UnityEngine;
using Zenject;

public sealed class ControllersInstaller : MonoInstaller
{
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private PostProcessingController _postProcessingController;   
    [SerializeField] private CameraZoomController _cameraZoomController;
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private HoverableController _hoverableController;
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private TimeController _timeController;
    [SerializeField] private DragController _dragController;
    [SerializeField] private GameController _gameController;
    
    public override void InstallBindings()
    { 
        Container.Bind<CameraRotationController>().FromInstance(_cameraRotationController).AsSingle().NonLazy();
        Container.Bind<CameraPositionController>().FromInstance(_cameraPositionController).AsSingle().NonLazy();
        Container.Bind<PostProcessingController>().FromInstance(_postProcessingController).AsSingle().NonLazy();
        Container.Bind<CameraZoomController>().FromInstance(_cameraZoomController).AsSingle().NonLazy();
        Container.Bind<InspectorController>().FromInstance(_inspectorController).AsSingle().NonLazy();
        Container.Bind<HoverableController>().FromInstance(_hoverableController).AsSingle().NonLazy();
        Container.Bind<CursorController>().FromInstance(_cursorController).AsSingle().NonLazy();
        Container.Bind<TimeController>().FromInstance(_timeController).AsSingle().NonLazy();
        Container.Bind<DragController>().FromInstance(_dragController).AsSingle().NonLazy();
        Container.Bind<GameController>().FromInstance(_gameController).AsSingle().NonLazy();
    }
}