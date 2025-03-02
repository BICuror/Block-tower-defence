using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]

public sealed class TMPLinkHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _targetTextField;
    [SerializeField] private Canvas _parentCanvas;
    
    private RectTransform _textBoxTransform;
    private int _currentLinkIndex;
    private Camera _camera;

    public Action<string> LinkClicked;
    public Action<string> LinkHovered;
    public Action LinkClosed;
    
    private void Awake()
    {
        _textBoxTransform = GetComponent<RectTransform>();
        _camera = _parentCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 mousePos = new Vector3(eventData.position.x, eventData.position.y, 0);
        
        int intersectingLinkIndex = TMP_TextUtilities.FindIntersectingLink(_targetTextField, mousePos, _camera);

        if (intersectingLinkIndex == -1) return;
        
        string linkKeyword = _targetTextField.textInfo.linkInfo[intersectingLinkIndex].GetLinkID();
        LinkClicked?.Invoke(linkKeyword);
    }

    private void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        
        if (!TMP_TextUtilities.IsIntersectingRectTransform(_textBoxTransform, mousePos, _camera)) return;
        
        int intersectingLinkIndex = TMP_TextUtilities.FindIntersectingLink(_targetTextField, mousePos, _camera);

        if (intersectingLinkIndex != -1)
        {
            if (intersectingLinkIndex == _currentLinkIndex) return;
            
            string linkKeyword = _targetTextField.textInfo.linkInfo[intersectingLinkIndex].GetLinkID();
            LinkHovered?.Invoke(linkKeyword);
        }
        else if (_currentLinkIndex != -1)
        {
            LinkClosed?.Invoke();
        }
        
        _currentLinkIndex = intersectingLinkIndex;
    }
}