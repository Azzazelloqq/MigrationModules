using System;
using Code.Core.CharactersControlModules.BaseModule;
using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.MVP;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP.Base
{
public interface IJoystickPresenter : IPresenter, ICharacterModule
{
    public event Action<JoystickAxis> AxisChanged; 
    public void ShowJoystick();
    public void HideJoystick();
    public void LockJoystick();
    public void UnlockJoystick();
    void OnJoystickMoved(Vector2 inputVector);
    void OnDrag(PointerEventData eventData);
    void OnEndDrag(PointerEventData eventData);
    void OnBeginDrag(PointerEventData eventData);
    public JoystickAxis GetAxis();
}
}