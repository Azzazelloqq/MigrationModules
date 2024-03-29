using UnityEngine;

namespace Code.Core.UIContext
{
//TODO: make UICanvasView and other MVP things
public class UnityUIContext : MonoBehaviour, IUIContext
{
    [field: SerializeField]
    public Canvas Canvas { get; private set; }

    [field: SerializeField]
    public RectTransform CanvasRectTransform { get; private set; }

    [field: SerializeField]
    public RectTransform UIOverlayParent { get; private set; }

    [field: SerializeField]
    public RectTransform PopupsParent { get; private set; }

    [field: SerializeField]
    public RectTransform FullscreenParent { get; private set; }

    [field: SerializeField]
    public RectTransform GameplayUIElements { get; private set; }

    [field: SerializeField]
    public RectTransform DevParent { get; private set; }

    [field: SerializeField]
    public RectTransform JoystickParent { get; private set; }
    
    [field: SerializeField]
    public RectTransform MainHud { get; private set; }

    [field: SerializeField]
    public RectTransform MiniMapParent { get; private set; }

    [field: SerializeField]
    public RectTransform TutorialsParent { get; private set; }
    
    [field: SerializeField]
    public RectTransform LoadingScreen { get; private set;  }

    private bool _isDestroyed;

    public void Dispose()
    {
        if (!_isDestroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }
}
}