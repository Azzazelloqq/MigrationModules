using System.Collections;
using Code.Core.CharacterAreaTriggers.ActivityTriggerArea.BaseMVP;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.ActivityTriggerArea
{
public class ActivityTriggerAreaView : ActivityTriggerAreaViewBase
{
    [field: SerializeField]
    public override Transform ZoneViewParent { get; protected set; }

    [SerializeField]
    private SpriteRenderer _progressBar;

    [SerializeField]
    private GameObject _areaFrame;

    public override GameObject GameObject => gameObject;

    private Coroutine _progressBarAnimationRoutine;
    private float _initializeProgressBarSizeX;
    private Vector3 _initialLocalPosition;
    private bool IsProgressBarHidden => !_progressBar.gameObject.activeSelf;

    public override void Initialize(IActivityTriggerAreaPresenter basePresenter)
    {
        base.Initialize(basePresenter);
        
        GetComponent<Collider>();
        InitializeProgressBar();
    }
    
    #region UnityMethods
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
    
    private void OnTriggerEnter(Collider other)
    {
        presenter?.OnAreaTriggerEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        presenter?.OnAreaTriggerExit(other.gameObject);
    }

    #endregion

    public override void StartActivityProgressAnimation(float activityDuration)
    {
        if (IsProgressBarHidden)
        {
            return;
        }
        
        CancelProgressBarAnimation();
        _progressBarAnimationRoutine = StartCoroutine(StartProgressBarAnimation(activityDuration));
    }

    public override void OnActivityCompleted()
    {
        SetBarProgressZero();
    }

    public override void CancelActivityAnimation()
    {
        CancelProgressBarAnimation();
    }

    public override void ShowProgressBar()
    {
        _progressBar.gameObject.SetActive(true);
    }

    public override void HideProgressBar()
    {
        _progressBar.gameObject.SetActive(false);
    }

    public override void ShowAreaFrame()
    {
        _areaFrame.SetActive(true);
    }

    public override void HideAreaFrame()
    {
        _areaFrame.SetActive(false);
    }

    public override Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public override void UpdateActivityCompletedState(bool isCompleted)
    {
        //_purposeActivity.SetActive(!isCompleted);
    }

    public override Vector3 GetAreaPosition()
    {
        return transform.position;
    }

    public void UpdateLockState(bool modelIsLock)
    {
        //_triggerCollider.enabled = !modelIsLock;
    }
    
    private void InitializeProgressBar()
    {
        var size = _progressBar.size;
        var barTransform = _progressBar.transform;
        _initialLocalPosition = barTransform.localPosition;
        
        _initializeProgressBarSizeX = size.x;
        
        SetBarProgressZero();
    }

    private IEnumerator StartProgressBarAnimation(float activityDuration)
    {
        var time = 0f;

        Vector2 startPosition = _progressBar.transform.localPosition;

        while (time < activityDuration)
        {
            var width = Mathf.Lerp(0, _initializeProgressBarSizeX, time / activityDuration);
            _progressBar.size = new Vector2(width, _progressBar.size.y);

            var positionOffset = width / 2;
            _progressBar.transform.localPosition = new Vector2(startPosition.x + positionOffset, startPosition.y);

            time += Time.deltaTime;
            yield return null;
        }

        _progressBar.size = new Vector2(_initializeProgressBarSizeX, _progressBar.size.y);
        _progressBar.transform.localPosition = new Vector2(startPosition.x + _initializeProgressBarSizeX / 2, startPosition.y);
    }

    private void CancelProgressBarAnimation()
    {
        if (_progressBarAnimationRoutine != null)
        {
            StopCoroutine(_progressBarAnimationRoutine);
        }

        SetBarProgressZero();
    }

    private void SetBarProgressZero()
    {
        var size = _progressBar.size;
        var progressBarTransform = _progressBar.transform;
        var localPosition = _initialLocalPosition;
        var startPositionX = localPosition.x - _initializeProgressBarSizeX / 2;
        progressBarTransform.localPosition = new Vector3(startPositionX, localPosition.y, localPosition.z);
        
        size = new Vector2(0, size.y);
        _progressBar.size = size;
    }
}
}