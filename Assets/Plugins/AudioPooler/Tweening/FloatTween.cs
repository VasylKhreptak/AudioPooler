using Plugins.AudioPooler.Tweening.Core;
using UnityEngine;

namespace Plugins.AudioPooler.Tweening
{
    public class FloatTween : Tween<float>
    {
        protected override float GetValueAtTime(float startValue, float targetValue, float time)
        {
            return Mathf.Lerp(startValue, targetValue, Evaluate(time));
        }
    }
}
