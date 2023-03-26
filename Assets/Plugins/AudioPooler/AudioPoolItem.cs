using System;
using DG.Tweening;
using Plugins.AudioPooler.Linker;
using UnityEngine;

namespace Plugins.AudioPooler
{
    public class AudioPoolItem : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioSettings settings;
        public PositionLinker linker;
        public Tween waitTween;
        public Tween volumeTween;
        public bool isPaused;
        public int ID = -1;

        public event Action<AudioPoolItem> onEnable;
        public event Action<AudioPoolItem> onDisable;

        public Action onPlay;
        public Action onPause;
        public Action onResume;
        public Action onStop;
        public Action onMute;
        public Action onUnmute;

        #region MonoBehaviour

        private void OnEnable()
        {
            onEnable?.Invoke(this);
        }

        private void OnDisable()
        {
            KillTweens();
            isPaused = false;

            onDisable?.Invoke(this);
        }

        #endregion

        private void KillTweens()
        {
            waitTween.Kill();
            volumeTween.Kill();
        }
    }
}
