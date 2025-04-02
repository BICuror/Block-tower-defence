using System;

namespace Combat
{
    public abstract class EntityEffect
    {
        protected ArgumentsContainer ArgumentsContainer;
        protected CombatEntity Entity;
        public int MaxStacks;
        public int TrueStack;
        public int Stack;

        public void Initialize(ArgumentsContainer argumentsContainer, int maxStacks)
        {
            ArgumentsContainer = argumentsContainer;
            MaxStacks = maxStacks;
            OnInitialized();
        }

        protected abstract void OnInitialized();
        
        public void SetEntity(CombatEntity entity) => Entity = entity;

        public void SetStack(int strength)
        {
            TrueStack = strength;
            Stack = strength;
        }
        public void ChangeStack(int strengthIncrease)
        {
            TrueStack += strengthIncrease;
            Stack = Math.Clamp(Stack + strengthIncrease, 0, MaxStacks);
        }
        
        public abstract void Update();
        public abstract void ApplyToEntity();
        public abstract void RemoveFromEntity();
    }
}