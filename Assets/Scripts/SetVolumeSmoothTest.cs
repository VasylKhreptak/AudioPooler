using DG.Tweening;
using Plugins.AudioPooler;
using Plugins.AudioPooler.Core;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

public class SetVolumeSmoothTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private AudioSettings _settings;
    [SerializeField] private Ease _ease = Ease.Linear;
    [SerializeField] private bool _stopOnComplete = false;

    private int _ID;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ID = _audioPooler.Play(_settings);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _audioPooler.SetVolumeSmooth(_ID, 0f, 2f, _stopOnComplete, _ease);
        }
    }

    #endregion
}
