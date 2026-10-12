using System.Collections.Generic;
using Combat.EnemySpawnSystem;
using UnityEngine;
using CuroAudio;
using System;

namespace Configs
{
    public enum ArgumentType
    {
        Undefined = 0,
        Int = 1,
        Float = 2,
        GameObject = 3,
        EntityModificatorData = 4,
        Bool = 5,
        EntityEffectParticleHandler = 6,
        EntityObjectModifier = 7,
        LayerSetting = 8,
        AdditionalEnemyGroup = 9,
        Sprite = 10,
        String = 11,
        StatusEffectConfig = 12,
        AudioEnum = 13,
        StatData = 14,
    }

    public static class ArgumentTypeLookupTable
    {
        public static Dictionary<Type, ArgumentType> Table = new()
        {
            {typeof(int), ArgumentType.Int},
            {typeof(float), ArgumentType.Float},
            {typeof(GameObject), ArgumentType.GameObject},
            {typeof(EntityModificatorData), ArgumentType.EntityModificatorData},
            {typeof(bool), ArgumentType.Bool},
            {typeof(EntityEffectParticleHandler), ArgumentType.EntityEffectParticleHandler},
            {typeof(EntityObjectModifier), ArgumentType.EntityObjectModifier},
            {typeof(LayerSetting), ArgumentType.LayerSetting},
            {typeof(AdditionalEnemyGroupDataConfig), ArgumentType.AdditionalEnemyGroup},
            {typeof(Sprite), ArgumentType.Sprite},
            {typeof(string), ArgumentType.String},
            {typeof(EntityEffectData), ArgumentType.StatusEffectConfig},
            {typeof(AudioEnum), ArgumentType.AudioEnum},
            {typeof(StatData), ArgumentType.StatData},
        };
    }
}