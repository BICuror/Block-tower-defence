using UnityEngine;
using UnityEngine.Events;

public sealed class OverheatManager : MonoBehaviour
{
    [SerializeField] private float _maxOverheat;
    
    private float _currentOverheat;

    [SerializeField] private float _coldownSpeed;

    private bool _isOverheated;

    public UnityEvent ReachedZeroOverheat;

    public bool IsOverheated() => _isOverheated;
    
    public void AddOverheat(float value)
    {
        _currentOverheat += value;

        if (_currentOverheat >= _maxOverheat)
        {
            _currentOverheat = _maxOverheat;

            _isOverheated = true;
        }            
    }

    private void FixedUpdate() 
    {
        if (_currentOverheat <= 0f)
        {
            _isOverheated = false;

            _currentOverheat = 0f;
        }
        else 
        {
            _currentOverheat -= _coldownSpeed;
        }
    }
}
