using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    public abstract class UpdatableScriptableObject : ScriptableObject
    {
        protected void OnValidate()
        {
            foreach (INonScriptableObjectUpdatable updatable in GetAllNonScriptableObjectUpdatables())
            {
                updatable.Update();
            }
        }

        protected abstract IEnumerable<INonScriptableObjectUpdatable> GetAllNonScriptableObjectUpdatables();
    }
}