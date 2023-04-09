using Plugins.AudioPooler;
using Plugins.AudioPooler.Core;
using UnityEngine;

public class StopPauseResumeAllTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plugins.AudioPooler.Core.AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private KeyCode _stopAllKeyCode;
    [SerializeField] private KeyCode _pauseAllKeyCode;
    [SerializeField] private KeyCode _resumeAllKeyCode;
    
    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<Plugins.AudioPooler.Core.AudioPooler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_stopAllKeyCode))
        {
            _audioPooler.StopAll();
        }
        
        if (Input.GetKeyDown(_pauseAllKeyCode))
        {
            _audioPooler.PauseAll();
        }
        
        if (Input.GetKeyDown(_resumeAllKeyCode))
        {
            _audioPooler.ResumeAll();
        }
    }
}
