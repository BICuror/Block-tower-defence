using UnityEngine;

namespace CuroAudio
{
    public sealed class AudioSystemEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            IAudioPlayer audioPlayer = new AudioSourceAudioPlayer();
            audioPlayer.Initialize();
            
            AudioSystem.Initialize(audioPlayer);
        }
    }
}

