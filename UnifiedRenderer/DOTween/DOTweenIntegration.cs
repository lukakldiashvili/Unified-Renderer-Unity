using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Unify.UnifiedRenderer.DOTween {
    public static class DOTweenIntegration {

        public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnifiedRenderer uniRend, string identifier, Color endColor, float duration, int matIndex = -1) {
            TweenerCore<Color, Color, ColorOptions> t = DG.Tweening.DOTween.To(() => {
                uniRend.TryGetPropertyValue<Color>(identifier, out var color, matIndex);
                return color;
            }, x => uniRend.TrySetPropertyValue(identifier, x, matIndex), endColor, duration);
            
            t.SetTarget(uniRend);
            return t;
        }
        
        public static TweenerCore<float, float, FloatOptions> DOFloat(this UnifiedRenderer uniRend, string identifier, float endValue, float duration, int matIndex = -1) {
            TweenerCore<float, float, FloatOptions> t = DG.Tweening.DOTween.To(() => {
                uniRend.TryGetPropertyValue<float>(identifier, out var floatVal, matIndex);
                return floatVal;
            }, x => {
                uniRend.TrySetPropertyValue(identifier, x, matIndex);
            }, endValue, duration);
            
            t.SetTarget(uniRend);
            return t;
        }

    }
}