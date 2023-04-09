using System;
using System.Collections;
using Plugins.AudioPooler.TimeManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.AudioPooler.Tweening.Core
{
    public abstract class Tween<T> where T : struct
    {
        private Coroutine _coroutine;

        private bool _isPaused;

        #region Settings

        private T? _from;
        private T _to;
        private Func<T> _getter;
        private Action<T> _setter;
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

        public Tween<T> Play()
        {
            if (_coroutine == null)
            {
                _isPaused = false;
                _coroutine = CoroutineHandler.Instance.StartCoroutine(TweenRoutine());
            }

            return this;
        }

        public Tween<T> Stop()
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

        public Tween<T> Pause()
        {
            _isPaused = true;
            return this;
        }

        public Tween<T> Resume()
        {
            _isPaused = false;
            return this;
        }

        public Tween<T> TogglePause()
        {
            _isPaused = !_isPaused;
            return this;
        }

        public Tween<T> Getter(Func<T> getter)
        {
            _getter = getter;
            return this;
        }

        public Tween<T> Setter(Action<T> setter)
        {
            _setter = setter;
            return this;
        }

        public Tween<T> From(T from)
        {
            _from = from;
            return this;
        }

        public Tween<T> To(T to)
        {
            _to = to;
            return this;
        }

        public Tween<T> Duration(float duration)
        {
            _duration = duration;
            return this;
        }

        public Tween<T> OnComplete(Action callback)
        {
            _onComplete = callback;
            return this;
        }

        public Tween<T> Curve(AnimationCurve curve)
        {
            _curve = curve;
            return this;
        }

        public Tween<T> Reset()
        {
            Stop();

            _from = null;
            _to = default;
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

            T from = _from ?? _getter?.Invoke() ?? default;

            float time = 0;
            while (time < _duration)
            {
                if (_isPaused == false)
                {
                    time += Time.deltaTime;
                    _setter?.Invoke(GetValueAtTime(from, _to, time / _duration));
                }

                yield return null;
            }

            _setter?.Invoke(_to);
            _coroutine = null;
            _onComplete?.Invoke();
        }

        protected abstract T GetValueAtTime(T startValue, T targetValue, float time);

        protected float Evaluate(float time)
        {
            if (_curve == null) return time;

            return _curve.Evaluate(time);
        }
    }
}
