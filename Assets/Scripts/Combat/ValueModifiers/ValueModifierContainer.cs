using System.Collections.Generic;
using System.Linq;
using System;

namespace Combat.ValueModifiers
{
    public sealed class ValueModifierContainer
    {
        private static readonly ValueModifierOrder[] _valueModifierOrder = new[]
        {
            ValueModifierOrder.PreDamageCalculation,
            ValueModifierOrder.EarlyDamageCalculation,
            ValueModifierOrder.DefaultDamageCalculation,
            ValueModifierOrder.PostDamageCalculation
        };
        
        private Dictionary<ValueModifierOrder, ListDictionary<Type, ValueModifier>> _modifiersDictionaryList;
        private CombatEntity _ownerEntity;

        public ValueModifierContainer(CombatEntity ownerEntity)
        {
            _modifiersDictionaryList = new();
            _ownerEntity = ownerEntity;
        }

        public void Add(ValueModifier modifier)
        {
            modifier.Initialize(_ownerEntity);

            if (!_modifiersDictionaryList.ContainsKey(modifier.Order)) _modifiersDictionaryList.Add(modifier.Order, new()); 
            
            _modifiersDictionaryList[modifier.Order].Add(modifier.GetType(), modifier);
        }

        public void Remove(ValueModifier modifier)
        {
            Type modifierType = modifier.GetType();

            _modifiersDictionaryList[modifier.Order].Remove(modifierType, modifier);
        }

        public bool TryModify(ValueModificationContainer container, CombatEntity otherEntity)
        {
            if (!container.AllowValuePass) return false;

            foreach (ValueModifierOrder valueModifierOrder in _valueModifierOrder)
            {
                if (_modifiersDictionaryList.ContainsKey(valueModifierOrder))
                {
                    ModifyByOrder(_modifiersDictionaryList[valueModifierOrder].GetAllItems());
                    
                    if (!container.AllowValuePass) return false;
                }
            }

            return true;
            
            void ModifyByOrder(IEnumerable<ValueModifier> modifiers)
            {
                float currentModifiedValue = container.GetModifiedValue();
                
                foreach (ValueModifier valueModifier in modifiers)
                {
                    if (!valueModifier.AllowValuePass(otherEntity, currentModifiedValue))
                    {
                        container.AllowValuePass = false;
                        return;
                    }
                
                    container.FlatChange += valueModifier.GetFlatChange(otherEntity, currentModifiedValue);
                    container.MultChange += valueModifier.GetMultChange(otherEntity, currentModifiedValue);
                }
            }
        }
    }

    public sealed class ValueModificationContainer
    {
        public float InitialValue;
        public float FlatChange;
        public float MultChange = 1;
        public bool AllowValuePass = true;

        public ValueModificationContainer(float initialValue)
        {
            InitialValue = initialValue;
        }
        
        public ValueModificationContainer() {}
        
        public float GetModifiedValue()
        {
            return (InitialValue + FlatChange) * MultChange;
        }
    }
}