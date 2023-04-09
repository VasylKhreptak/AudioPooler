using NaughtyAttributes;
using Plugins.AudioPooler.Core;
using UnityEngine;
using UnityEngine.Audio;

public class SetTrackVolumeTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private string _name;
    [SerializeField, Range(0f, 1f)] private float _volume;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
    }

    #endregion

    [Button("Update Volume")]
    private void UpdateVolume()
    {
        _audioPooler.SetTrackVolume01(_audioMixer, _name, _volume);
    }
}
