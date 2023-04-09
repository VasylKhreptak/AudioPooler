using Plugins.AudioPooler.Core;
using UnityEngine;

public class SetSpatialBlendSmooth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private Plugins.AudioPooler.Data.AudioSettings _settings;
    [SerializeField] private bool _stopOnComplete = false;
    [SerializeField] private float _from = 0f;
    [SerializeField] private float _to = 1f;

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
            _audioPooler.SetSpatialBlend(_ID, _from);
            _audioPooler.SetSpatialBlendSmooth(_ID, _to, 2f, _stopOnComplete);
        }

        if (Input.GetMouseButtonDown(2))
        {
            _audioPooler.StopUpdatingSpatialBlend(_ID);
        }
    }

    #endregion
}
