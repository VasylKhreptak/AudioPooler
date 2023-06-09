using Plugins.AudioPooler;
using Plugins.AudioPooler.Core;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

public class PlayPauseResumeStopTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plugins.AudioPooler.Core.AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private AudioSettings _settings;
    [SerializeField] private KeyCode _stop;

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
            _audioPooler.Pause(_ID);
        }

        if (Input.GetMouseButtonDown(2))
        {
            _audioPooler.Resume(_ID);
        }

        if (Input.GetKeyDown(_stop))
        {
            _audioPooler.Stop(_ID);
        }
    }

    #endregion
}
