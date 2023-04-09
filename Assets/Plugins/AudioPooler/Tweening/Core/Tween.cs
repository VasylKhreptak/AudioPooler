using System;
using System.Collections;
using Plugins.AudioPooler.TimeManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.AudioPooler.Tweening.Core
{
    public class Tween
    {
        private Coroutine _coroutine;

        private bool _isPaused;

        #region Settings

        private float? _from;
        private float _to;
        private Func<float> _getter;
        private Action<float> _setter;
        private float _duration;
        private AnimationCurve _curve;

        #endregion

        #region Callbacks

        private Action _onComplete;

        #endregion

        #region Data

        public bool IsPaused => _isPaused;

        public bool IsActive => _coroutine != null;

        public bool IsPlaying => IsActive && _isPaused == false;

        #endregion

        #region Interface

        public Tween Play()
        {
            if (_coroutine == null)
            {
                _isPaused = false;
                _coroutine = CoroutineHandler.Instance.StartCoroutine(TweenRoutine());
            }

            return this;
        }

        public Tween Stop()
        {
            if (_coroutine != null && SceneManager.GetActiveScene().isLoaded)
            {
                CoroutineHandler.Instance.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            return this;
        }

        public void Restart()
        {
            Stop();
            Play();
        }

        public Tween Pause()
        {
            _isPaused = true;
            return this;
        }

        public Tween Resume()
        {
            _isPaused = false;
            return this;
        }

        public Tween TogglePause()
        {
            _isPaused = !_isPaused;
            return this;
        }

        public Tween Getter(Func<float> getter)
        {
            _getter = getter;
            return this;
        }

        public Tween Setter(Action<float> setter)
        {
            _setter = setter;
            return this;
        }

        public Tween From(float from)
        {
            _from = from;
            return this;
        }

        public Tween To(float to)
        {
            _to = to;
            return this;
        }

        public Tween Duration(float duration)
        {
            _duration = duration;
            return this;
        }

        public Tween OnComplete(Action callback)
        {
            _onComplete = callback;
            return this;
        }

        public Tween Curve(AnimationCurve curve)
        {
            _curve = curve;
            return this;
        }

        public Tween Reset()
        {
            Stop();

            _from = null;
            _to = 0;
            _getter = null;
            _setter = null;
            _duration = 0;
            _curve = null;
            _onComplete = null;

            return this;
        }

        #endregion

        private IEnumerator TweenRoutine()
        {
            if (_from != null) _setter?.Invoke(_from.Value);

            float from = _from ?? _getter?.Invoke() ?? 0;

            float time = 0;
            while (time < _duration)
            {
                if (_isPaused == false)
                {
                    time += Time.deltaTime;
                    _setter?.Invoke(Mathf.Lerp(from, _to, Evaluate(time / _duration)));
                }

                yield return null;
            }

            _setter?.Invoke(_to);
            _coroutine = null;
            _onComplete?.Invoke();
        }

        private float Evaluate(float time)
        {
            if (_curve == null) return time;

            return _curve.Evaluate(time);
        }
    }
}
