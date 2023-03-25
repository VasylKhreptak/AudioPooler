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
        public int ID = -1;

        public event Action<AudioPoolItem> onEnable;
        public event Action<AudioPoolItem> onDisable;

        #region MonoBehaviour

        private void OnEnable()
        {
            onEnable?.Invoke(this);
        }

        private void OnDisable()
        {
            onDisable?.Invoke(this);
        }

        #endregion
    }
}
