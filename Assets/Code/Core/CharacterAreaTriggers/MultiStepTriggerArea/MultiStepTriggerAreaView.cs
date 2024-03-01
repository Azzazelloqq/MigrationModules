using System;
using Code.Core.CharacterAreaTriggers.MultistepTriggerArea.BaseMVP;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Core.CharacterAreaTriggers.MultiStepTriggerArea
{
public class MultiStepTriggerAreaView : MultiStepTriggerAreaViewBase
{
    private bool IsProgressBarHidden => !progressBarView.gameObject.activeSelf;
    
    [FormerlySerializedAs("_progressBar")]
    [SerializeField]
    private MultiStepTriggerProgressBarView progressBarView;
    
    private void OnTriggerEnter(Collider other)
    {
        presenter?.OnAreaTriggerEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        presenter?.OnAreaTriggerExit(other.gameObject);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        progressBarView.Dispose();
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        var trigger = GetComponent<Collider>();

        if (trigger == null)
        {
            Debug.LogWarning("Need collider");
            return;
        }
        
        if (!trigger.isTrigger)
        {
            trigger.isTrigger = true;
        }
    }
    #endif
    
   
    public override void InitializeProgressBar(float startValue = 0f)
    {
        progressBarView.Initialize(startValue);
    }
    
    public override void UpdateTriggerProgressBar(float startValue, float endValue, float activityDuration)
    {
        progressBarView.UpdateBarValue(startValue, endValue, activityDuration);
    }

    public override void SetStepFilledCallback(Action callback)
    {
        progressBarView.OnFillComplete(callback);
    }
    
    public override void OnStepProcessCompleted()
    {
        
    }

    public override void CancelUnlockAnimation()
    {
    }

    public override void ShowProgressBar()
    {
        progressBarView.gameObject.SetActive(true);
    }

    public override void HideProgressBar()
    {
        progressBarView.gameObject.SetActive(false);
    }

    public override void ShowAreaFrame()
    {
        
    }

    public override void HideAreaFrame()
    {
        
    }

    public override Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public override void UpdateMultiStepTriggerCompletedState(bool isCompleted)
    {
        
    }

    public override Vector3 GetTriggerAreaPosition()
    {
        return transform.position;
    }
    
}
}