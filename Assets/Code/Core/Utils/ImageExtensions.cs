using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Utils
{
public static class ImageExtensions
{
    public static void SetAlpha(this Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    public static TweenerCore<Color, Color, ColorOptions> DoAlpha(this Image image, float alpha, float duration)
    {
        var newColor = new Color(image.color.r, image.color.g, image.color.b, alpha);
        var tweenerCore = image.DOColor(newColor, duration);

        return tweenerCore;
    }
}
}