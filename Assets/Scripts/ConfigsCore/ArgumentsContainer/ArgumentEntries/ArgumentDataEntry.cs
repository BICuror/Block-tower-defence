using Combat.Entity.Modificators;
using Combat.EnemySpawnSystem;
using Combat.ValueModifiers;
using NaughtyAttributes;
using UnityEngine;
using CuroAudio;
using System;

namespace Configs
{
    [Serializable] public struct ArgumentDataEntry
    {
        [HideInInspector] public string Name;
        [HideInInspector] public ArgumentType ArgumentType;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.Int)]
        public int IntValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.Float)]
        public float FloatValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.GameObject)]
        public GameObject GameObjectValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.EntityModificatorConfig)]
        public EntityModificatorConfig EntityModificatorConfigValue;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.Bool)]
        public bool BoolValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.EntityEffectParticleHandler)]
        public EntityEffectParticleHandler EntityEffectParticleHandlerValue;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.EntityObjectModifier)]
        public EntityObjectModifier EntityObjectModifierValue;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.LayerSetting)]
        public LayerSetting LayerSettingValue;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.AdditionalEnemyGroup)]
        public AdditionalEnemyGroupDataConfig AdditionalEnemyGroupValue;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.Sprite)]
        public Sprite SpriteValue;

        [AllowNesting, ShowIf("ArgumentType", ArgumentType.String)]
        public string StringValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.StatusEffectConfig)]
        public EntityEffectConfig StatusEffectConfigValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.AudioEnum)]
        public AudioEnum AudioEnumgValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.StatConfig)]
        public StatConfig StatConfigValue;
        
        [AllowNesting, ShowIf("ArgumentType", ArgumentType.ValueModifierConfig)]
        public ValueModifierConfig ValueModifierConfigValue;
        
        public object GetValue(Type argumentType)
        {
            ArgumentType = ArgumentTypeLookupTable.Table[argumentType];

            switch (ArgumentType)
            {
                case ArgumentType.Int: return IntValue;
                case ArgumentType.Float: return FloatValue;
                case ArgumentType.GameObject: return GameObjectValue;
                case ArgumentType.EntityModificatorConfig: return EntityModificatorConfigValue;
                case ArgumentType.Bool: return BoolValue;
                case ArgumentType.EntityEffectParticleHandler: return EntityEffectParticleHandlerValue;
                case ArgumentType.EntityObjectModifier: return EntityObjectModifierValue;
                case ArgumentType.LayerSetting: return LayerSettingValue;
                case ArgumentType.AdditionalEnemyGroup: return AdditionalEnemyGroupValue;
                case ArgumentType.Sprite: return SpriteValue;
                case ArgumentType.String: return StringValue;
                case ArgumentType.StatusEffectConfig: return StatusEffectConfigValue;
                case ArgumentType.AudioEnum: return AudioEnumgValue;
                case ArgumentType.StatConfig: return StatConfigValue;
                case ArgumentType.ValueModifierConfig: return ValueModifierConfigValue;
                default: throw new ArgumentException($"Not implemented argument of type {ArgumentType}");
            }
        }
    }
}