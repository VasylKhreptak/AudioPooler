using DG.Tweening;
using UnityEngine;

namespace Plugins.AudioPooler.Extensions
{
    public static class DOTweenExtensions
    {
        public static Tween DOWait(this MonoBehaviour onwner, float delay)
        {
            return DOTween.To(() => 0, x => {}, 1, delay).Play();
        }
    }
}
