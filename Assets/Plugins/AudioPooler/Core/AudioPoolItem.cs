using System;
using JetBrains.Annotations;
using Plugins.AudioPooler.Linker;
using Plugins.AudioPooler.TimeManagement;
using Plugins.AudioPooler.Tweening;
using Plugins.AudioPooler.Tweening.Core;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

namespace Plugins.AudioPooler.Core
{
    public class AudioPoolItem : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioSettings settings;
        public PositionLinker linker;
        public readonly Timer timer = new Timer();
        
        public bool isPaused;
        public int ID = -1;

        #region Tweens

        public readonly FloatTween volumeTween = new FloatTween();
        public readonly FloatTween pitchTween = new FloatTween();
        public readonly FloatTween spatialBlendTween = new FloatTween();
        public readonly FloatTween stereoPanTween = new FloatTween();
        public readonly FloatTween reverbZoneMixTween = new FloatTween();
        public readonly FloatTween spreadTween = new FloatTween();
        public readonly FloatTween minDistanceTween = new FloatTween();
        public readonly FloatTween maxDistanceTween = new FloatTween();

        #endregion

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
            timer.Stop();
            volumeTween.Stop();
        }
    }
}
