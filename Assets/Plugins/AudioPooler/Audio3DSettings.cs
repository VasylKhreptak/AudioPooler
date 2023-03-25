using System;
using UnityEngine;

namespace Plugins.AudioPooler
{
    [Serializable]
    public class Audio3DSettings
    {
        [Range(0f, 5f)] public float dopplerLevel = 1f;
        [Range(0f, 360f)] public int spread;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        public float minDistance = 1f;
        public float maxDistance = 500f;
    }
}
