using System.Collections;
using UnityEngine;

namespace Plugins.AudioPooler.TimeManagement
{
    public class Timer
    {
        private Coroutine _coroutine;

        private bool _isPaused;

        #region Interface

        public void Start(float duration, System.Action callback)
        {
            if (_coroutine == null)
            {
                _coroutine = DelayHandler.Instance.StartCoroutine(TimerRoutine(duration, callback));
            }
        }

        public void Stop()
        {
            if (_coroutine != null)
            {
                DelayHandler.Instance.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        public void Restart(float duration, System.Action callback)
        {
            Stop();
            Start(duration, callback);
        }

        public void Pause() => _isPaused = true;

        public void Resume() => _isPaused = false;

        public void TogglePause() => _isPaused = !_isPaused;

        #endregion

        private IEnumerator TimerRoutine(float duration, System.Action callback)
        {
            float time = 0;
            while (time < duration)
            {
                if (_isPaused == false)
                {
                    time += Time.deltaTime;
                }
                
                yield return null;
            }
            
            _coroutine = null;
            callback?.Invoke();
        }
    }
}
