using Plugins.AudioPooler;
using UnityEngine;

public class AudioPoolerTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private int _mouseKeyIndex;
    [Space]
    [SerializeField] private Plugins.AudioPooler.AudioSettings _settings;

    private Camera _camera;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
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
