using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using Cashing;
using Zenject;
using System;
using Combat;

public sealed class ConnectAllBuildingsInArea : EntityObjectModifier
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private CombatEntity _owner;
    
    [SerializeField] private AreaEntityDetector _areaDetector;
    [SerializeField] private LayerSetting _enemyLayerSetting;
    [SerializeField] private BeamSystem _beamSystemPrefab;
    [SerializeField] private WeaponBase _weapon;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private Dictionary<BuildingEntity, BeamSystem> _createdBeamSystems = new();
    private float _beamDamagePeriod;
    private float _beamDamage;

    private bool BeamShouldBeActive => _buildingDraggable.IsBuilt && _waveStateMachine.CurrentState == WaveState.Attack;
    
    private void Start()
    {
        _weapon.Initialize(_owner);
        
        _beamDamage = Args.GetArgument<float>("BeamDamage");
        _beamDamagePeriod = Args.GetArgument<float>("BeamDamagePeriod");

        _buildingDraggable.BuildCompleted += InitializeBeams;
        _buildingDraggable.PickedUp += StopBeams;

        _areaDetector.AddedItem += TryAddBuilding;
        _areaDetector.RemovedItem += TryRemoveBuilding;

        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted += DestroyAllBeamSystems;
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted += InitializeBeams;
    }

    private void InitializeBeams()
    {
        if (BeamShouldBeActive)
        {
            CreateAllBeamSystems();
            StartBeamDamageCycle().Forget();
        }
    }
    
    private void StopBeams()
    {
        DestroyAllBeamSystems();
        StopBeamDamageCycle();
    }

    private void TryAddBuilding(CombatEntity combatEntity)
    {
        if (BeamShouldBeActive) CreateBeamSystem(combatEntity as BuildingEntity);
    }

    private void TryRemoveBuilding(CombatEntity combatEntity)
    {
        if (BeamShouldBeActive) DestroyBeam(combatEntity as BuildingEntity);
    }
    
    #region DamageLogic

    private async UniTask StartBeamDamageCycle()
    {
        while (BeamShouldBeActive)
        {
            try
            {
                await UniTask.WaitForSeconds(_beamDamagePeriod, cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                break;
            }

            DamageAllEntitiesInBeamArea();
        }
    }

    private void DamageAllEntitiesInBeamArea()
    {
        Vector3 startPosition = transform.position;
        
        foreach (BuildingEntity buildingInArea in _createdBeamSystems.Keys)
        {
            Vector3 endPosition = buildingInArea.transform.position;
            
            Ray ray = new Ray(startPosition, endPosition - startPosition);
            
            float rayLength = Vector3.Distance(startPosition, endPosition);

            RaycastHit[] raycastHits = Physics.RaycastAll(ray, rayLength, _enemyLayerSetting.GetLayerMask());
            
            foreach (RaycastHit raycastHit in raycastHits)
            {
                if (raycastHit.collider.gameObject.TryGetComponent(out CombatEntity enemyEntity))
                {
                    _weapon.DamageEntity(_beamDamage, enemyEntity);
                }
            }
        }
    }
    
    private void StopBeamDamageCycle()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    #endregion

    #region BeamVisuals

    private void CreateAllBeamSystems()
    {
        foreach (BuildingEntity buildingInArea in _areaDetector.GetList())
        {
            CreateBeamSystem(buildingInArea);
        }
    }

    private void CreateBeamSystem(BuildingEntity buildingEntity)
    {
        if (_createdBeamSystems.ContainsKey(buildingEntity)) return;
        
        BeamSystem beamSystem = Instantiate(_beamSystemPrefab, transform.position, Quaternion.identity, transform);
        
        beamSystem.SetSource(transform);
        beamSystem.SetTarget(buildingEntity.transform);
        
        _createdBeamSystems.Add(buildingEntity, beamSystem);
    }
    
    private void DestroyAllBeamSystems()
    {
        List<BuildingEntity> buildingEntities = _createdBeamSystems.Keys.ToList();
        
        for (int i = 0; i < buildingEntities.Count; i++)
        {
            DestroyBeam(buildingEntities[i]);
        }
        
        _createdBeamSystems.Clear();
    }

    private void DestroyBeam(BuildingEntity buildingEntity)
    {
        if (!_createdBeamSystems.ContainsKey(buildingEntity)) return;
        
        BeamSystem associatedBeamSystem = _createdBeamSystems[buildingEntity];
        
        _createdBeamSystems.Remove(buildingEntity);
        
        associatedBeamSystem.DisableBeam();
        Destroy(associatedBeamSystem.gameObject);
    }

    #endregion

    private void OnDisable()
    {
        DestroyAllBeamSystems();
    }

    private void OnDestroy()
    {
        DestroyAllBeamSystems();
        
        _buildingDraggable.BuildCompleted -= InitializeBeams;
        _buildingDraggable.PickedUp -= StopBeams;

        _areaDetector.AddedItem -= TryAddBuilding;
        _areaDetector.RemovedItem -= TryRemoveBuilding;
        
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted -= DestroyAllBeamSystems;
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted -= InitializeBeams;
    }
}