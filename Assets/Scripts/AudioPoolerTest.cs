using Plugins.AudioPooler;
using Plugins.AudioPooler.Core;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

public class AudioPoolerTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plugins.AudioPooler.Core.AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private int _mouseKeyIndex;
    [Space]
    [SerializeField] private AudioSettings _settings;

    private Camera _camera;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<Plugins.AudioPooler.Core.AudioPooler>();
    }

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(_mouseKeyIndex))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                PlaySound(hitInfo.point);
            }
        }
    }

    #endregion

    private void PlaySound(Vector3 position)
    {
        _settings.playPosition = position;
        _audioPooler.Play(_settings);
    }
}
