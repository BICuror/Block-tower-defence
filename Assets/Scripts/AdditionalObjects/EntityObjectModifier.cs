using UnityEngine;
using Combat;

public abstract class EntityObjectModifier : MonoBehaviour
{
    protected ArgumentsContainer Args;
    
    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public virtual bool CanBeAppliedToEntity(CombatEntity entity) => true;
}