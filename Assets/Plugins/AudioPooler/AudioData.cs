using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.AudioPooler
{
    [Serializable]
    public class AudioData
    {
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        public bool loop;
        [Range(0, 256)] public int priority = 128;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
        [Range(-1f, 1f)] public float stereoPan;
        [Range(0f, 1f)] public float spatialBlend;
        [Range(0f, 1.1f)] public float reverbZoneMix = 1f;
        public Audio3DData audio3DData;
    }
}
