using System;
using Plugins.AudioPooler;
using Plugins.AudioPooler.Core;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

public class ManualLinkerTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private KeyCode _startLinking = KeyCode.A;
    [SerializeField] private KeyCode _stopLinking = KeyCode.D;

    [Space]
    [SerializeField] private AudioSettings _settings;

    private int _ID;

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
    }

    private void Start()
    {
        _ID = _audioPooler.Play(_settings);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_startLinking))
        {
            _audioPooler.StartLinking(_ID);
        }

        if (Input.GetKeyDown(_stopLinking))
        {
            _audioPooler.StopLinking(_ID);
        }
    }
}
