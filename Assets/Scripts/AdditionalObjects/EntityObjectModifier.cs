using UnityEngine;
using Combat;

public abstract class EntityObjectModifier : MonoBehaviour
{
    public virtual bool CanBeAppliedToEntity(CombatEntity entity) => true;
}