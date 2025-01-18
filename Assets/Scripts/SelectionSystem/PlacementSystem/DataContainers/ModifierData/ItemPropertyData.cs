using UnityEngine;

[CreateAssetMenu(fileName = "ItemPropertyData", menuName = "Item/ItemPropertyData")]

public sealed class ItemPropertyData : ItemModifierData
{
    [Header("Duration")]
    [SerializeField] [Range(1f, 5f)] private float _minDuration = 1f;
    [SerializeField] [Range(1f, 5f)] private float _maxDuration = 1f;
    
    public float MinDuration => _minDuration;
    public float MaxDuration => _maxDuration;
}