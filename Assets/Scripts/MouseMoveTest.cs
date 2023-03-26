using System.Collections;
using Plugins.AudioPooler;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.AudioSettings;

public class MouseMoveTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioPooler _audioPooler;

    [Space]
    [SerializeField] private AudioSettings _settings;

    private Coroutine _moveCoroutine;

    private int _audioID;

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
        if (Input.GetMouseButtonDown(0))
        {
            StartMoving();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopMoving();
        }
    }

    #endregion

    private void StartMoving()
    {
        if (_moveCoroutine == null)
        {
            _audioID = _audioPooler.Play(_settings);

            _moveCoroutine = StartCoroutine(MoveRoutine());
        }
    }

    private void StopMoving()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);

            _moveCoroutine = null;

            _audioPooler.Stop(_audioID);
        }
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            Move();

            yield return null;
        }
    }

    private void Move()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            _audioPooler.SetPosition(_audioID, hitInfo.point);
        }
    }
}
