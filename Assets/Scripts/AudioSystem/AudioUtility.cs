using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace CuroAudio
{
    public static class AudioUtility
    {
        public static readonly string GENERATED_CODE_PATH = "Assets/Scripts/AudioSystem";
        private static readonly string PATH_TO_MAIN_AUDIO_CONFIG = "Audio/AudioMainContainer";
        private const char SEPARATOR_SYMBOL = '_';
    
        public static AudioSystemConfig GetAudioSystemConfig()
        {
            return Resources.Load<AudioSystemConfig>(PATH_TO_MAIN_AUDIO_CONFIG);
        }
        
        public static string GetAudioReferenceFullID(string segmentName, string subSegmentName, string soundID)
        {
            return $"{segmentName}{SEPARATOR_SYMBOL}{subSegmentName}{SEPARATOR_SYMBOL}{soundID}".ToLower();
        }

        public static async UniTask DoVirtual(float startValue, float endValue, float duration, Action<float> onUpdate)
        {
            onUpdate(startValue);
            
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                await UniTask.WaitForFixedUpdate();
                
                elapsedTime += Time.fixedDeltaTime;
                
                onUpdate.Invoke(Mathf.Lerp(startValue, endValue, elapsedTime / duration));
            }
            
            onUpdate(endValue);
        }
    }
}