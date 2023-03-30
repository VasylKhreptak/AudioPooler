using System;
using Plugins.AudioPooler.Linker;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Plugins.AudioPooler
{
    [Serializable]
    public class AudioSettings
    {
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        public bool loop;
        public bool suspendOnLowVolume = true;
        [Range(0, 256)] public int priority = 128;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
        [Range(-1f, 1f)] public float stereoPan;
        [Range(0f, 1f)] public float spatialBlend = 1f;
        [Range(0f, 1.1f)] public float reverbZoneMix = 1f;
        [Space]
        public bool linkOnPlay = true;
        public LinkerData linkerData;
        public Vector3 playPosition;
        [Space]
        public Audio3DSettings audio3DSettings = new Audio3DSettings();
    }
}
