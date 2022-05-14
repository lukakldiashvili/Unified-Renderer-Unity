using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Unify.UnifiedRenderer.DOTween {
    public static class DOTweenIntegration {

        public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnifiedRenderer uniRend, string identifier, int matIndex, Color endColor, float duration) {
            TweenerCore<Color, Color, ColorOptions> t = DG.Tweening.DOTween.To(() => {
                uniRend.TryGetPropertyValue<Color>(identifier, out var color, matIndex);
                return color;
            }, x => uniRend.TrySetPropertyValue(identifier, x, matIndex), endColor, duration);
            
            t.SetTarget(uniRend);
            return t;
        }

    }
}