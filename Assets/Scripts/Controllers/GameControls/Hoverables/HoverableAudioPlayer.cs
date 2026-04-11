using UnityEngine;
using CuroAudio;

[RequireComponent(typeof(HoverableObject))]

public sealed class HoverableAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioEnum _audioClipToPlayOnHoverEnter;
    [SerializeField] private AudioEnum _audioClipToPlayOnHoverExit;
    
    private void Awake()
    {
        HoverableObject hoverableObject = GetComponent<HoverableObject>();

        if (_audioClipToPlayOnHoverEnter != AudioEnum.Undefined) hoverableObject.HoverEntered.AddListener(PlayOnHoverEnterSFX);
        if (_audioClipToPlayOnHoverExit != AudioEnum.Undefined) hoverableObject.HoverEntered.AddListener(PlayOnHoverExitSFX);
    }

    private void PlayOnHoverEnterSFX()
    {
        AudioSystem.PlaySFX(_audioClipToPlayOnHoverEnter, transform.position);
    }    
    
    private void PlayOnHoverExitSFX()
    {
        AudioSystem.PlaySFX(_audioClipToPlayOnHoverEnter, transform.position);
    }
}