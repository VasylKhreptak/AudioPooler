using System.Collections;
using Plugins.AudioPooler;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.AudioSettings;

public class MassivePlayTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _transform;
    [SerializeField] private AudioPooler _audioPooler;

    [Header("Preferences")]
    [SerializeField] private KeyCode _start = KeyCode.F1;
    [SerializeField] private KeyCode _stop = KeyCode.F2;
    [SerializeField] private float _delay;
    [SerializeField] private float _range;

    [Space]
    [SerializeField] private AudioSettings _settings;

    private Coroutine _playCoroutine;

    #region MonoBehaviour

    private void OnValidate()
    {
        _audioPooler ??= FindObjectOfType<AudioPooler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_start))
        {
            if (_playCoroutine != null) return;

            _playCoroutine = StartCoroutine(PlayingRoutine());
        }

        if (Input.GetKeyDown(_stop))
        {
            if (_playCoroutine == null) return;

            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
    }

    #endregion

    private IEnumerator PlayingRoutine()
    {
        while (true)
        {
            _settings.playPosition = GetRandomPosition();

            _audioPooler.Play(_settings);

            yield return new WaitForSeconds(_delay);
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle * _range;
        Vector3 randomdirection = new Vector3(insideUnitCircle.x, _transform.position.y, insideUnitCircle.y);

        return _transform.position + randomdirection;
    }

    private void OnDrawGizmos()
    {
        if (_transform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_transform.position, _range);
    }
}
