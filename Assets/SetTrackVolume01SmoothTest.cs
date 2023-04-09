using NaughtyAttributes;
using Plugins.AudioPooler.Core;
using UnityEngine;
using UnityEngine.Audio;

public class SetTrackVolume01SmoothTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;
    [SerializeField] private AudioMixer _mixer;

    [Header("Preferences")]
    [SerializeField] private string _name;
    [SerializeField] private float _startVolume = 0;
    [SerializeField] private float _targetVolume = 1;
    [SerializeField] private float _time = 2;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
    }

    #endregion

    [Button("Update Volume")]
    private void UpdateVolume()
    {
        _audioPooler.SetTrackVolume01(_mixer, _name, _startVolume);
        _audioPooler.SetTrackVolume01Smooth(_mixer, _name, _targetVolume, _time);
    }
}
