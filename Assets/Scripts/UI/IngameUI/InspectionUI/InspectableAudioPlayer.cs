using UnityEngine;
using CuroAudio;

[RequireComponent(typeof(InspectableObject))]

public sealed class InspectableAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioEnum _audioClipToPlayOnInspectionStart;
    [SerializeField] private AudioEnum _audioClipToPlayOnInspectionEnd;
    
    private void Awake()
    {
        InspectableObject inspectableObject = GetComponent<InspectableObject>();

        if (_audioClipToPlayOnInspectionStart != AudioEnum.Undefined) inspectableObject.InspectionStarted += PlayOnInspectionStartSFX;
        if (_audioClipToPlayOnInspectionEnd != AudioEnum.Undefined) inspectableObject.InspectionEnded += PlayOnInspectionEndSFX;
    }
    
    private void PlayOnInspectionStartSFX()
    {
        AudioSystem.PlaySFX(_audioClipToPlayOnInspectionStart, transform.position);
    }    
    
    private void PlayOnInspectionEndSFX()
    {
        AudioSystem.PlaySFX(_audioClipToPlayOnInspectionEnd, transform.position);
    }
}