using UnityEngine;

public class CombatBuilding : MonoBehaviour
{
    [SerializeField] private float _baseDamage;

    public float Damage => _baseDamage;

    public virtual string GetDamageValue() => _baseDamage.ToString();
}
