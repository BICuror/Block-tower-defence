using UnityEngine;
using CuroAudio;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(DraggableObject))]

public sealed class DraggableAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioEnum _audioClipToPlayOnPickedUp;
    [SerializeField] private AudioEnum _audioClipToPlayOnPlaced;
    [SerializeField] private AudioEnum _audioClipToPlayOnDragged;
    private DraggableObject _draggableObject;
    
    private void Awake()
    {
        _draggableObject = GetComponent<DraggableObject>();

        if (_audioClipToPlayOnPickedUp != AudioEnum.Undefined) _draggableObject.PickedUp += PlayOnPickedUpSFX;
        if (_audioClipToPlayOnPlaced != AudioEnum.Undefined) _draggableObject.Placed += PlayOnPlacedSFX;
        if (_audioClipToPlayOnDragged != AudioEnum.Undefined) _draggableObject.PickedUp += StartPlayingDragSFXLoop;
    }

    private void PlayOnPickedUpSFX()
    {
        AudioSystem.PlaySFX(_audioClipToPlayOnPickedUp, transform.position);
    }    
    
    private void PlayOnPlacedSFX()
    {
        AudioSystem.PlaySFX(_audioClipToPlayOnPlaced, transform.position);
    }
    
    private void StartPlayingDragSFXLoop() => PlayDragSFXLoop().Forget();
    private async UniTask PlayDragSFXLoop()
    {
        AudioSource source = await AudioSystem.PlayLoopSFX(_audioClipToPlayOnDragged);
        
        float volume = source.volume;

        await DOVirtual.Float(0f, volume, 0.7f, value => source.volume = value).AsyncWaitForCompletion();
        
        source.transform.parent = transform;
        source.transform.localPosition = Vector3.zero;

        await UniTask.WaitWhile(() => !_draggableObject.IsPlaced);
        
        await DOVirtual.Float(volume, 0f, 0.7f, value => source.volume = value).AsyncWaitForCompletion();
        
        source.Stop();
    }
}