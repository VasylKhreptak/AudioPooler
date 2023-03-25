using UnityEngine;

namespace Plugins.AudioPooler
{
    public class AudioPooler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _transform;

        [Header("Preferences")]
        [SerializeField] private AudioData _defaultSettings;

        [Header("Pool Preference")]
        [SerializeField] private int _initialSize;
        [SerializeField] private bool _autoExpand;
        [SerializeField] private int _maxSize;

        #region MonoBehaviour

        private void OnValidate()
        {
            _transform ??= GetComponent<Transform>();

            ValidateInputData();
        }

        #endregion

        #region DataValidation

        private void ValidateInputData()
        {
            ValidateInitialSize();

            ValidateMaxSize();
        }

        private void ValidateInitialSize()
        {
            if (_initialSize < 1)
            {
                _initialSize = 1;
            }
        }

        private void ValidateMaxSize()
        {
            if (_maxSize < _initialSize)
            {
                _maxSize = _initialSize;
            }
        }

        #endregion
    }
}
