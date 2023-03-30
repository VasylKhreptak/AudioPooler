using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        Debug.Log(_audioSource.gamepadSpeakerOutputType);
    }
}
