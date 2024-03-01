using System;
using UnityEngine;

namespace Code.Core.UIContext
{
public interface IUIContext : IDisposable
{
    public Canvas Canvas { get; }
    public RectTransform CanvasRectTransform { get; }
    public RectTransform UIOverlayParent { get; }
    public RectTransform PopupsParent { get; }
    public RectTransform FullscreenParent { get; }
    public RectTransform JoystickParent { get; }
    public RectTransform GameplayUIElements { get; }
    public RectTransform DevParent { get; }
    public RectTransform MainHud { get; }
    public RectTransform MiniMapParent { get; }
}
}