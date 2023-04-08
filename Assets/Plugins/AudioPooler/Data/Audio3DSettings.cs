using System;
using Plugins.AudioPooler.Enums;
using UnityEngine;

namespace Plugins.AudioPooler.Data
{
    [Serializable]
    public class Audio3DSettings
    {
        [Range(0f, 5f)] public float dopplerLevel = 1f;
        [Range(0f, 360f)] public int spread;
        public RolloffMode rolloffMode = RolloffMode.Linear;
        public float minDistance = 1f;
        public float maxDistance = 500f;
    }
}
