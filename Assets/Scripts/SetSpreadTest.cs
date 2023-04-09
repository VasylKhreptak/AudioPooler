using Plugins.AudioPooler.Core;
using UnityEngine;

public class SetSpreadTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plugins.AudioPooler.Core.AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private Plugins.AudioPooler.Data.AudioSettings _settings;
    [SerializeField] private bool _stopOnComplete = false;
    [SerializeField] private float _from = 0f;
    [SerializeField] private float _to = 1f;

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
            _audioPooler.SetSpread(_ID, _from);
            _audioPooler.SetSpreadSmooth(_ID, _to, 2f, _stopOnComplete);
        }

        if (Input.GetMouseButtonDown(2))
        {
            _audioPooler.StopUpdatingSpread(_ID);
        }
    }

    #endregion
}
