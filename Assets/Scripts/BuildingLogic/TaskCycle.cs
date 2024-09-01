using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TaskCycle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _rechargeTime;
    public float RechargeTime 
    {
        get => _rechargeTime;  
        
        set 
        {
            _rechargeTime = value;

            _rechargeInstruction = new WaitForSeconds(value);
        } 
    }

    private bool _taskCycleIsActive;
    
    public UnityEvent TaskPerformed;

    public delegate bool ShouldWork();
    public ShouldWork ShouldWorkDelegate;

    private YieldInstruction _rechargeInstruction;

    protected void Awake() => _rechargeInstruction = new WaitForSeconds(_rechargeTime);

    public void StartCycle() => Recharge();
    
    private void Recharge()
    {
        if (CanWork() && ShouldWorkDelegate() && _taskCycleIsActive == false)
        {
            _taskCycleIsActive = true;

            StartCoroutine(StartRechargeProcess());
        }
    }
    
    public virtual bool CanWork() => true;

    private IEnumerator StartRechargeProcess()
    {
        yield return _rechargeInstruction;
        
        _taskCycleIsActive = false;

        if (ShouldWorkDelegate())
        {
            Recharge();
            
            PerformTask();
        }
    }
    
    private void PerformTask() => TaskPerformed?.Invoke();
    
    public void StopCycle()  
    {
        _taskCycleIsActive = false;

        StopAllCoroutines();
    }
}
