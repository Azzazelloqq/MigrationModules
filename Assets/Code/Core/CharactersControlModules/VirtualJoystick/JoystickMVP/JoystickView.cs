using Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP.Base;
using Code.Core.MVP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP
{
public class JoystickView : JoystickViewBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image _joystickHandle;

    [SerializeField]
    private Image _joystickBackground;

    [SerializeField]
    private RectTransform _parent;
    
    private Vector3 _initializeBackgroundPosition;
    private bool _isLocked;

    public override void Initialize(IJoystickPresenter presenter)
    {
        base.Initialize(presenter);
        
        InitializeJoystickDefaultPosition();
    }

    public override void SetJoystickActive(bool isActive)
    {
        _parent.gameObject.SetActive(isActive);
    }

    public override void SetLockState(bool isLock)
    {
        _isLocked = isLock;
    }

    public override void SetJoystickToStartPosition()
    {
        _joystickHandle.transform.localPosition = Vector3.zero;
        SetJoystickDefaultPosition();
        presenter.OnJoystickMoved(Vector2.zero);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsCanUse())
        {
            return;
        }

        presenter.OnBeginDrag(eventData);
        
        MoveJoystickToBeginDrag(eventData);

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsCanUse())
        {
            return;
        }

        presenter.OnDrag(eventData);
        
        UpdateHandlePosition(eventData, out var axis);

        presenter.OnJoystickMoved(axis);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        presenter.OnEndDrag(eventData);
         SetJoystickDefaultPosition();
    }

    private void SetJoystickDefaultPosition()
    {
        _joystickBackground.rectTransform.position = _initializeBackgroundPosition;
        _joystickHandle.rectTransform.localPosition = Vector3.zero;
    }

    private void InitializeJoystickDefaultPosition()
    {
        _initializeBackgroundPosition = _joystickBackground.rectTransform.position;
    }

    private void MoveJoystickToBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _joystickBackground.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var newPosition);
        
        _joystickBackground.rectTransform.position = new Vector3(
            newPosition.x,
            newPosition.y,
            _joystickBackground.rectTransform.position.z);
    }

    private void UpdateHandlePosition(PointerEventData eventData, out Vector2 axis)
    {
        var backgroundPosition = new Vector2(_joystickBackground.rectTransform.position.x,
            _joystickBackground.rectTransform.position.y);
        var handleTransform = _joystickHandle.rectTransform;
        
        var dragPosition = GetDragPosition(eventData);

        var localDragPosition = _joystickBackground.transform.InverseTransformPoint(dragPosition);
        axis = (dragPosition - backgroundPosition).normalized;

        var radius = GetJoystickRadius();
        var distance = Vector2.Distance(Vector2.zero, localDragPosition);

        if (distance < radius)
        {
            handleTransform.localPosition = new Vector3(localDragPosition.x, localDragPosition.y, handleTransform.localPosition.z);
        }
        else
        {
            var newPositionWithOffset = axis * radius;
            handleTransform.localPosition = new Vector3(newPositionWithOffset.x, newPositionWithOffset.y, handleTransform.localPosition.z);
        }
    }

    private Vector2 GetDragPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_joystickBackground.rectTransform, eventData.position,
            eventData.pressEventCamera, out var newPosition);

        return newPosition;
    }

    private float GetJoystickRadius()
    {
        var backgroundSizeDelta = _joystickBackground.rectTransform.sizeDelta;
        var radius = backgroundSizeDelta.x / 2;

        return radius;
    }

    private bool IsCanUse()
    {
        return !_isLocked;
    }
}
}