using NaughtyAttributes;
using Plugins.AudioPooler.Core;
using UnityEngine;
using UnityEngine.Audio;

public class FromDbTo01Test : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private float _Db = -80;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
    }

    #endregion

    [Button("Update Volume")]
    private void UpdateVolume()
    {
        Debug.Log(_audioPooler.FromDbTo01(_Db));
    }
}
