using System.Collections.Generic;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine;
using Zenject;
using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public sealed class EnviromentController : MonoBehaviour
{
    [Inject] private PostProcessingController _postProcessingController;
    [Inject] private WaveStateMachine _waveStateMachine;

    [SerializeField] private AnimationCurve _changeCurve;
    [SerializeField] private List<EnviromentState> _enviromentStates;
    [SerializeField] private float _changeDuration;

    [Header("Rain")] 
    [SerializeField] private ParticleSystem _rainParticles;
    [SerializeField] private float _rainChance;
    
    private int _currentEnviromentStateIndex;

    private void Awake()
    {
        _waveStateMachine.StateEnded += _ => TryEnterNextState();
        
        _currentEnviromentStateIndex = Random.Range(0, _enviromentStates.Count);

        SetEnviromentState(_enviromentStates[_currentEnviromentStateIndex]);
    }

    [Button] public void DebugChangeEnviromentState() => TryEnterNextState();

    private void SetEnviromentState(EnviromentState state)
    {
        state.EnviromentLightChanges.ForEach(lightChange =>
        {
            lightChange.Light.intensity = lightChange.Intensity;
            lightChange.Light.color = lightChange.Color;
        });

        state.EnviromentMaterialChanges.ForEach(materialChange =>
        {
            materialChange.MeshRenderer.sharedMaterial = new Material(materialChange.Material);
            
            materialChange.PropertyNames.ForEach(propertyName =>
            {
                materialChange.MeshRenderer.sharedMaterial.SetColor(propertyName, materialChange.Color);
            });
        });

        _postProcessingController.SetProfile(state.VolumeProfile);
        
        UpdateRainState();
    }

    private void TryEnterNextState()
    {
        MoveToNextState();
        
        _enviromentStates[_currentEnviromentStateIndex].EnviromentLightChanges.ForEach(lightChange =>
        {
            lightChange.Light.DOIntensity(lightChange.Intensity, _changeDuration).SetEase(_changeCurve);
            lightChange.Light.DOColor(lightChange.Color, _changeDuration).SetEase(_changeCurve);
            lightChange.Light.transform.DORotate(new Vector3(Random.Range(20f, 50f), Random.Range(0f, 360f), 0f), _changeDuration);
        });

        _enviromentStates[_currentEnviromentStateIndex].EnviromentMaterialChanges.ForEach(DoMaterialPropertyTween);
        
        _postProcessingController.ChangeCustomVolume(_enviromentStates[_currentEnviromentStateIndex].VolumeProfile, _changeDuration).Forget();

        UpdateRainState();
    }

    private void UpdateRainState()
    {
        if (Random.Range(0, 100) < _rainChance) _rainParticles.Play();
        else _rainParticles.Stop();
    }

    private void MoveToNextState()
    {
        _currentEnviromentStateIndex++;

        if (_currentEnviromentStateIndex >= _enviromentStates.Count) _currentEnviromentStateIndex = 0;
    }
    
    private void DoMaterialPropertyTween(EnviromentMaterialChange materialChange)
    {
        Color initialColor = materialChange.MeshRenderer.sharedMaterial.GetColor(materialChange.PropertyNames[0]);
        
        DOVirtual.Float(0, 1, _changeDuration,
            (value) =>
            {
                materialChange.PropertyNames.ForEach(propertyName =>
                {
                    materialChange.MeshRenderer.sharedMaterial.SetColor(propertyName, Color.Lerp(initialColor, materialChange.Color, value));
                });
            });
    }

    [Serializable] private sealed class EnviromentState
    {
        [SerializeField] private List<EnviromentMaterialChange> _enviromentMaterialChanges;
        [SerializeField] private List<EnviromentLightChange> _enviromentLightChanges;
        [SerializeField] private VolumeProfile _volumeProfile;
        
        public List<EnviromentMaterialChange> EnviromentMaterialChanges => _enviromentMaterialChanges;
        public List<EnviromentLightChange> EnviromentLightChanges => _enviromentLightChanges;
        public VolumeProfile VolumeProfile => _volumeProfile;
    }

    [Serializable] private sealed class EnviromentMaterialChange
    {
        [SerializeField] private List<string> _propertyNames;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _material;
        [SerializeField] private Color _color;
        
        public List<string> PropertyNames => _propertyNames;  
        public MeshRenderer MeshRenderer => _meshRenderer;  
        public Material Material => _material;
        public Color Color => _color;
    }
    
    [Serializable] private sealed class EnviromentLightChange
    {
        [SerializeField] private Light _light;
        [SerializeField] private float _intensity;
        [SerializeField] private Color _color;
        
        public Light Light => _light;   
        public float Intensity => _intensity;
        public Color Color => _color;
    }
}