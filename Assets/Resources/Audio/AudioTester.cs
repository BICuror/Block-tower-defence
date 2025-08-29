using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CuroAudio;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class AudioTester : MonoBehaviour
{
    private async void Awake()
    {
        while (true)
        {
            AudioSystem.PlayMusic(AudioEnum.music_ui_main_menu, AudioLayer.Main);
            
            await UniTask.WaitForSeconds(10);
            
            
            
            await UniTask.WaitForSeconds(10);
        }
    }
}
