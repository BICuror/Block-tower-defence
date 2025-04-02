using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Navigation;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyDatas/EnemyData")]

public sealed class EnemyData : ScriptableObject
{
    [Header("Navigation")] 
    [SerializeField] private float _speed = 2f;
    [SerializeField] private NavigationAgentData _navigationData;

    [Header("Health")]
    [SerializeField] private float _maxHealth = 25f;
    [Range(0.01f, 3f)] [SerializeField] private float _incomingDamageMultipluer = 1f;

    [Header("VisualSettings")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    
    [Header("Modificators")]
    [SerializeField] private bool _hasObjectModificators;
    [ShowIf("HasObjectModificators")] [SerializeField] private List<GameObject> _objectModificators;
    
    public float Speed => _speed;
    public NavigationAgentData NavigationData => _navigationData;
    public float MaxHealth => _maxHealth;
    public float IncomingDamageMultipluer => _incomingDamageMultipluer;
    public bool HasObjectModificators => _hasObjectModificators;
    public List<GameObject> ObjectModificators => _objectModificators;
    
    public Mesh Mesh => _mesh;
    public Material Material => _material;
}
