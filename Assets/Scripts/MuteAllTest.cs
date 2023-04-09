using Plugins.AudioPooler;
using Plugins.AudioPooler.Core;
using UnityEngine;

public class MuteAllTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plugins.AudioPooler.Core.AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private KeyCode _muteKeyCode = KeyCode.M;
    [SerializeField] private bool _mute;

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<Plugins.AudioPooler.Core.AudioPooler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_muteKeyCode))
        {
            _audioPooler.MuteAll(_mute);
        }
    }
}
