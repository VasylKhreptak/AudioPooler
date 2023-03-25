using System.Collections;
using Plugins.ObjectPooler.Extensions;
using UnityEngine;

namespace Plugins.AudioPooler.Linker
{
    public class PositionLinker : MonoBehaviour
    {
        private LinkerData _data;

        private Coroutine _linkCoroutine;

        private bool _isLinking;

        public bool IsLinking => _isLinking;

        public void StartUpdating(LinkerData data)
        {
            if (_linkCoroutine != null) return;

            _data = data;

            if (IsDataValid() == false)
            {
                return;
            }

            _linkCoroutine = StartCoroutine(LinkRoutine());
            _isLinking = true;
        }

        public void StopUpdating()
        {
            if (_linkCoroutine != null)
            {
                StopCoroutine(_linkCoroutine);

                _linkCoroutine = null;

                _data = null;

                _isLinking = false;
            }
        }

        #region MonoBehaviour

        private void OnDisable()
        {
            StopUpdating();
        }

        #endregion

        #region DataValidation

        private bool IsDataValid()
        {
            if (_data == null || _data.linkTo == null || _data.axes.Is01() == false)
            {
                return false;
            }

            return true;
        }

        #endregion

        private IEnumerator LinkRoutine()
        {
            while (true)
            {
                if (CanContinueLink())
                {
                    Link();
                }
                else
                {
                    StopUpdating();
                }

                yield return null;
            }
        }

        private bool CanContinueLink()
        {
            if (_data.linkTo == null)
            {
                Debug.LogWarning("Target is null.Cannot continue linking!Stopped linking!");
                return false;
            }

            return true;
        }

        private void Link()
        {
            transform.position = transform.position.ReplaceWithAxes(_data.axes, _data.linkTo.position + _data.offset);
        }
    }
}
