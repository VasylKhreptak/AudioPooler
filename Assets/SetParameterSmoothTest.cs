using NaughtyAttributes;
using Plugins.AudioPooler.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SetParameterSmoothTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;
    [SerializeField] private AudioMixer _mixer;

    [Header("Preferences")]
    [SerializeField] private string _name;
    [SerializeField] private float _startValue = 0;
    [SerializeField] private float _targetValue = 1;
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
        _audioPooler.SetParameter(_mixer, _name, _startValue);
        _audioPooler.SetParameterSmooth(_mixer, _name, _targetValue, _time);
    }
}
