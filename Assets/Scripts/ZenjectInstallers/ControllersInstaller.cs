using GameControls.Controllers;
using GameControls.Features;
using GameControls;
using UnityEngine;
using Zenject;

public sealed class ControllersInstaller : MonoInstaller
{
    [Header("Controllers")]
    [SerializeField] private CameraRotationController _cameraRotationController;
    [SerializeField] private PostProcessingController _postProcessingController;
    [SerializeField] private InspectorController _inspectorController;
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DragController _dragController;
    [SerializeField] private GameController _gameController;
    [SerializeField] private TimeController _timeController;
    
    [Header("Features")]
    [SerializeField] private CameraKeyboardRepositionGameControllerFeature _cameraKeyboardRepositionGameControllerFeature;
    [SerializeField] private CameraZoomGameControllerFeature _cameraZoomGameControllerFeature;
    [SerializeField] private HoverableGameControllerFeature _hoverableGameControllerFeature;
    [SerializeField] private TimeGameControllerFeature _timeGameControllerFeature;
    
    public override void InstallBindings()
    { 
        Container.Bind<CameraRotationController>().FromInstance(_cameraRotationController).AsSingle().NonLazy();
        Container.Bind<PostProcessingController>().FromInstance(_postProcessingController).AsSingle().NonLazy();
        Container.Bind<InspectorController>().FromInstance(_inspectorController).AsSingle().NonLazy();
        Container.Bind<CursorController>().FromInstance(_cursorController).AsSingle().NonLazy();
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle().NonLazy();
        Container.Bind<TimeController>().FromInstance(_timeController).AsSingle().NonLazy();
        Container.Bind<DragController>().FromInstance(_dragController).AsSingle().NonLazy();
        Container.Bind<GameController>().FromInstance(_gameController).AsSingle().NonLazy();
        
        Container.Bind<CameraKeyboardRepositionGameControllerFeature>().FromInstance(_cameraKeyboardRepositionGameControllerFeature).AsSingle().NonLazy();
        Container.Bind<CameraZoomGameControllerFeature>().FromInstance(_cameraZoomGameControllerFeature).AsSingle().NonLazy();
        Container.Bind<HoverableGameControllerFeature>().FromInstance(_hoverableGameControllerFeature).AsSingle().NonLazy();
        Container.Bind<TimeGameControllerFeature>().FromInstance(_timeGameControllerFeature).AsSingle().NonLazy();
    }
}