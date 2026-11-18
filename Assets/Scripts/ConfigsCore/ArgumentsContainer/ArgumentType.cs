using System.Collections.Generic;
using Combat.Entity.Modificators;
using Combat.EnemySpawnSystem;
using UnityEngine;
using CuroAudio;
using System;
using Combat.ValueModifiers;

namespace Configs
{
    public enum ArgumentType
    {
        Undefined = 0,
        Int = 1,
        Float = 2,
        GameObject = 3,
        EntityModificatorConfig = 4,
        Bool = 5,
        EntityEffectParticleHandler = 6,
        EntityObjectModifier = 7,
        LayerSetting = 8,
        AdditionalEnemyGroup = 9,
        Sprite = 10,
        String = 11,
        StatusEffectConfig = 12,
        AudioEnum = 13,
        StatConfig = 14,
        ValueModifierConfig = 15,
    }

    public static class ArgumentTypeLookupTable
    {
        public static Dictionary<Type, ArgumentType> Table = new()
        {
            {typeof(int), ArgumentType.Int},
            {typeof(float), ArgumentType.Float},
            {typeof(GameObject), ArgumentType.GameObject},
            {typeof(EntityModificatorConfig), ArgumentType.EntityModificatorConfig},
            {typeof(bool), ArgumentType.Bool},
            {typeof(EntityEffectParticleHandler), ArgumentType.EntityEffectParticleHandler},
            {typeof(EntityObjectModifier), ArgumentType.EntityObjectModifier},
            {typeof(LayerSetting), ArgumentType.LayerSetting},
            {typeof(AdditionalEnemyGroupDataConfig), ArgumentType.AdditionalEnemyGroup},
            {typeof(Sprite), ArgumentType.Sprite},
            {typeof(string), ArgumentType.String},
            {typeof(EntityEffectConfig), ArgumentType.StatusEffectConfig},
            {typeof(AudioEnum), ArgumentType.AudioEnum},
            {typeof(StatConfig), ArgumentType.StatConfig},
            {typeof(ValueModifierConfig), ArgumentType.ValueModifierConfig},
        };
    }
}