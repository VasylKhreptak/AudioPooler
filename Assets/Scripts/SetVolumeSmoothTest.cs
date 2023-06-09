using Plugins.AudioPooler.Core;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

public class SetVolumeSmoothTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plugins.AudioPooler.Core.AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private AudioSettings _settings;
    [SerializeField] private bool _stopOnComplete = false;

    private int _ID;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<Plugins.AudioPooler.Core.AudioPooler>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ID = _audioPooler.Play(_settings);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _audioPooler.SetVolumeSmooth(_ID, 0f, 2f, _stopOnComplete);
        }
        
        if (Input.GetMouseButtonDown(2))
        {
            _audioPooler.StopUpdatingVolume(_ID);
        }
    }

    #endregion
}
