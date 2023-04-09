using System.Collections.Generic;
using System.Linq;
using Plugins.AudioPooler.Data;
using Plugins.AudioPooler.Enums;
using Plugins.AudioPooler.Linker;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

namespace Plugins.AudioPooler.Core
{
    public class AudioPooler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _transform;

        [Header("Preferences")]
        [SerializeField, Min(0)] private float _volumeAmplifier = 1f;

        [Header("Pool Preference")]
        [SerializeField] private int _initialSize;
        [SerializeField] private bool _autoExpand;
        [SerializeField] private int _maxSize;
        [SerializeField] private bool _allocateMaxMemory;

        private AudioSettings _defaultSettings;

        private HashSet<AudioPoolItem> _pool;
        private Dictionary<int, AudioPoolItem> _activePool;
        private HashSet<AudioPoolItem> _inactivePool;

        private int _idGiver;

        private int CollectionCapacity => _allocateMaxMemory ? _maxSize : _initialSize;

        private Transform _audioListenerTransform;

        private const float MIN_PITCH = 0.1f;

        #region MonoBehaviour

        private void OnValidate()
        {
            _transform ??= GetComponent<Transform>();

            ValidateInputData();
        }

        private void Awake()
        {
            _defaultSettings = new AudioSettings();

            SetAudioListener(FindObjectOfType<AudioListener>());

            InitPool();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        #endregion

        #region Init

        private void InitPool()
        {
            int collectionCapacity = CollectionCapacity;

            _pool = new HashSet<AudioPoolItem>(collectionCapacity);
            _activePool = new Dictionary<int, AudioPoolItem>(collectionCapacity);
            _inactivePool = new HashSet<AudioPoolItem>(collectionCapacity);

            for (int i = 0; i < _initialSize; i++)
            {
                AddNewPoolItem();
            }
        }

        private AudioPoolItem AddNewPoolItem()
        {
            AudioPoolItem poolItem = CreatePoolItem(_defaultSettings);
            AddPoolItem(poolItem);

            return poolItem;
        }

        private void AddPoolItem(AudioPoolItem poolItem)
        {
            _pool.Add(poolItem);
            _inactivePool.Add(poolItem);

            AddListener(poolItem);
        }

        private AudioPoolItem CreatePoolItem(AudioSettings settings)
        {
            GameObject poolItemGO = new GameObject("AudioPoolItem");
            poolItemGO.SetActive(false);

            poolItemGO.transform.SetParent(_transform);

            AudioPoolItem poolItem = poolItemGO.AddComponent<AudioPoolItem>();
            poolItem.audioSource = poolItemGO.AddComponent<AudioSource>();
            poolItem.linker = poolItemGO.AddComponent<PositionLinker>();

            poolItem.audioSource.playOnAwake = false;
            poolItem.ID = -1;

            ApplySettings(poolItem, settings);

            return poolItem;
        }

        private void ApplySettings(AudioPoolItem poolItem, AudioSettings settings)
        {
            poolItem.settings = settings;
            poolItem.audioSource.clip = settings.clip;
            poolItem.audioSource.outputAudioMixerGroup = settings.audioMixerGroup;
            poolItem.audioSource.mute = settings.mute;
            poolItem.audioSource.bypassEffects = settings.bypassEffects;
            poolItem.audioSource.bypassListenerEffects = settings.bypassListenerEffects;
            poolItem.audioSource.bypassReverbZones = settings.bypassReverbZones;
            poolItem.audioSource.loop = settings.loop;
            poolItem.audioSource.priority = settings.priority;
            poolItem.audioSource.volume = settings.volume * _volumeAmplifier;
            poolItem.audioSource.pitch = ClampPitch(settings.pitch);
            poolItem.audioSource.panStereo = settings.stereoPan;
            poolItem.audioSource.spatialBlend = settings.spatialBlend;
            poolItem.audioSource.reverbZoneMix = settings.reverbZoneMix;

            Apply3DSettings(poolItem, settings.audio3DSettings);
        }

        private void Apply3DSettings(AudioPoolItem poolItem, Audio3DSettings settings)
        {
            poolItem.audioSource.dopplerLevel = settings.dopplerLevel;
            poolItem.audioSource.spread = settings.spread;
            poolItem.audioSource.rolloffMode = settings.rolloffMode == RolloffMode.Linear ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic;
            poolItem.audioSource.minDistance = settings.minDistance;
            poolItem.audioSource.maxDistance = settings.maxDistance;
        }

        #endregion

        #region ActiveInactivePoolManagement

        private void OnPoolItemEnabled(AudioPoolItem poolItem)
        {
            RemoveFromInactivePool(poolItem);
            AddToActivePool(poolItem);
        }

        private void OnPoolItemDisabled(AudioPoolItem poolItem)
        {
            RemoveFromActivePool(poolItem);
            AddToInactivePool(poolItem);
        }

        private void RemoveFromActivePool(AudioPoolItem poolItem) => _activePool.Remove(poolItem.ID);

        private void AddToActivePool(AudioPoolItem poolItem) => _activePool.Add(poolItem.ID, poolItem);

        private void RemoveFromInactivePool(AudioPoolItem poolItem) => _inactivePool.Remove(poolItem);

        private void AddToInactivePool(AudioPoolItem poolItem) => _inactivePool.Add(poolItem);

        private void AddListener(AudioPoolItem poolItem)
        {
            poolItem.onEnable += OnPoolItemEnabled;
            poolItem.onDisable += OnPoolItemDisabled;
        }

        private void RemoveListener(AudioPoolItem poolItem)
        {
            poolItem.onEnable -= OnPoolItemEnabled;
            poolItem.onDisable -= OnPoolItemDisabled;
        }

        private void AddListeners()
        {
            foreach (AudioPoolItem poolItem in _pool)
            {
                AddListener(poolItem);
            }
        }

        private void RemoveListeners()
        {
            foreach (AudioPoolItem poolItem in _pool)
            {
                RemoveListener(poolItem);
            }
        }

        #endregion

        #region Interface

        public int Play(AudioSettings settings)
        {
            if (CanPlay(settings) == false) return -1;

            AudioPoolItem poolItem = GetPoolItem();

            poolItem.ID = _idGiver++;

            ApplySettings(poolItem, settings);

            poolItem.transform.position = settings.playPosition;

            poolItem.gameObject.SetActive(true);

            if (settings.linkOnPlay)
            {
                poolItem.linker.StartUpdating(settings.linkerData);
            }

            Play(poolItem);

            if (settings.loop == false)
            {
                StopDelayed(poolItem);
            }

            return poolItem.ID;
        }

        public void Pause(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                Pause(poolItem);
            }
        }

        public void Resume(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem) && poolItem.audioSource.isPlaying == false)
            {
                Resume(poolItem);
            }
        }

        public void Stop(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                Stop(poolItem);
            }
        }

        public bool IsPlaying(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                return poolItem.audioSource.isPlaying;
            }

            return false;
        }

        public void SetBypassEffects(int ID, bool bypassEffects)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.bypassEffects = bypassEffects;
            }
        }

        public void SetBypassListenerEffects(int ID, bool bypassListenerEffects)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.bypassListenerEffects = bypassListenerEffects;
            }
        }

        public void SetBypassReverbZones(int ID, bool bypassReverbZones)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.bypassReverbZones = bypassReverbZones;
            }
        }

        public void SetVolume(int ID, float volume)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.volume = volume * _volumeAmplifier;
            }
        }

        public void SetVolumeSmooth(int ID, float volume, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.volumeTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.volume)
                    .Setter(x => poolItem.audioSource.volume = x)
                    .To(volume)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingVolume(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.volumeTween.Stop();
            }
        }

        public void SetPitch(int ID, float pitch)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.pitch = ClampPitch(pitch);
                poolItem.timer.UpdateDuration(GetDuration(poolItem.audioSource));
            }
        }

        public void SetPitchSmooth(int ID, float pitch, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            pitch = ClampPitch(pitch);

            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.pitchTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.pitch)
                    .Setter(x =>
                    {
                        poolItem.audioSource.pitch = x;
                        poolItem.timer.UpdateDuration(GetDuration(poolItem.audioSource));
                    })
                    .To(pitch)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingPitch(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.pitchTween.Stop();
            }
        }

        public void SetStereoPan(int ID, float stereoPan)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.panStereo = stereoPan;
            }
        }

        public void SetStereoPanSmooth(int ID, float stereoPan, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.stereoPanTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.panStereo)
                    .Setter(x => poolItem.audioSource.panStereo = x)
                    .To(stereoPan)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingStereoPan(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.stereoPanTween.Stop();
            }
        }

        public void SetSpatialBlend(int ID, float spatialBlend)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.spatialBlend = spatialBlend;
            }
        }

        public void SetSpatialBlendSmooth(int ID, float spatialBlend, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.spatialBlendTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.spatialBlend)
                    .Setter(x => poolItem.audioSource.spatialBlend = x)
                    .To(spatialBlend)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingSpatialBlend(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.spatialBlendTween.Stop();
            }
        }

        public void SetReverbZoneMix(int ID, float reverbZoneMix)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.reverbZoneMix = reverbZoneMix;
            }
        }

        public void SetReverbZoneMixSmooth(int ID, float reverbZoneMix, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.reverbZoneMixTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.reverbZoneMix)
                    .Setter(x => poolItem.audioSource.reverbZoneMix = x)
                    .To(reverbZoneMix)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingReverbZoneMix(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.reverbZoneMixTween.Stop();
            }
        }

        public void SetDopplerLevel(int ID, float dopplerLevel)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.dopplerLevel = dopplerLevel;
            }
        }

        public void SetSpread(int ID, float spread)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.spread = spread;
            }
        }

        public void SetSpreadSmooth(int ID, float spread, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.spreadTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.spread)
                    .Setter(x => poolItem.audioSource.spread = x)
                    .To(spread)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingSpread(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.spreadTween.Stop();
            }
        }

        public void SetMinDistance(int ID, float minDistance)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.minDistance = minDistance;
            }
        }

        public void SetMinDistanceSmooth(int ID, float minDistance, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.minDistanceTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.minDistance)
                    .Setter(x => poolItem.audioSource.minDistance = x)
                    .To(minDistance)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingMinDistance(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.minDistanceTween.Stop();
            }
        }

        public void SetMaxDistance(int ID, float maxDistance)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.maxDistance = maxDistance;
            }
        }

        public void SetMaxDistanceSmooth(int ID, float maxDistance, float time, bool stopOnComplete = false, AnimationCurve curve = null)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.maxDistanceTween
                    .Reset()
                    .Getter(() => poolItem.audioSource.maxDistance)
                    .Setter(x => poolItem.audioSource.maxDistance = x)
                    .To(maxDistance)
                    .Duration(time)
                    .Curve(curve)
                    .OnComplete(() =>
                    {
                        if (stopOnComplete) Stop(poolItem);
                    })
                    .Play();
            }
        }

        public void StopUpdatingMaxDistance(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.maxDistanceTween.Stop();
            }
        }

        public void Mute(int ID, bool mute)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.audioSource.mute = mute;
            }
        }

        public void MuteAll(bool mute)
        {
            foreach (AudioPoolItem poolItem in _activePool.Values.ToArray())
            {
                Mute(poolItem, mute);
            }
        }

        public void StopAll()
        {
            foreach (AudioPoolItem poolItem in _activePool.Values.ToArray())
            {
                Stop(poolItem);
            }
        }

        public void PauseAll()
        {
            foreach (AudioPoolItem poolItem in _activePool.Values.ToArray())
            {
                Pause(poolItem);
            }
        }

        public void ResumeAll()
        {
            foreach (AudioPoolItem poolItem in _activePool.Values.ToArray())
            {
                Resume(poolItem);
            }
        }

        public void StartLinking(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.linker.StartUpdating(poolItem.settings.linkerData);
            }
        }

        public void StopLinking(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.linker.StopUpdating();
            }
        }

        public void SetPosition(int ID, Vector3 worldPosition)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                poolItem.transform.position = worldPosition;
            }
        }

        public Transform GetTransform(int ID)
        {
            if (_activePool.TryGetValue(ID, out AudioPoolItem poolItem))
            {
                return poolItem.transform;
            }

            return null;
        }

        public void SetAudioListener(AudioListener listener)
        {
            _audioListenerTransform = listener.transform;
        }

        #endregion

        #region Core

        private bool CanPlay(AudioSettings settings)
        {
            return IsPlayVolumeRelevant(settings);
        }

        private bool IsPlayVolumeRelevant(AudioSettings settings)
        {
            if (settings.suspendOnDistance == false) return true;

            if (Mathf.Approximately(settings.volume, 0f)) return false;

            if (Mathf.Approximately(settings.spatialBlend, 1f) == false) return true;

            float distance = Vector3.Distance(_audioListenerTransform.position, settings.playPosition);

            if (settings.audio3DSettings.rolloffMode == RolloffMode.Linear)
            {
                return distance <= settings.audio3DSettings.maxDistance;
            }

            return true;
        }

        private void Play(AudioPoolItem poolItem)
        {
            poolItem.audioSource.Play();
            poolItem.onPlay?.Invoke();
            poolItem.isPaused = false;
        }

        private void Stop(AudioPoolItem poolItem)
        {
            if (poolItem.gameObject.activeSelf == false) return;

            poolItem.timer.Stop();
            poolItem.volumeTween.Stop();
            poolItem.audioSource.Stop();
            poolItem.linker.StopUpdating();
            poolItem.onStop?.Invoke();
            poolItem.gameObject.SetActive(false);
            poolItem.isPaused = false;
        }

        private void Pause(AudioPoolItem poolItem)
        {
            if (poolItem.isPaused) return;

            poolItem.timer.Pause();
            poolItem.volumeTween.Pause();
            poolItem.audioSource.Pause();
            poolItem.onPause?.Invoke();
            poolItem.isPaused = true;
        }

        private void Resume(AudioPoolItem poolItem)
        {
            if (poolItem.audioSource.isPlaying || poolItem.isPaused == false) return;

            if (poolItem.settings.loop == false)
            {
                poolItem.timer.Resume();
            }

            poolItem.audioSource.Play();
            poolItem.volumeTween.Resume();
            poolItem.onResume?.Invoke();
            poolItem.isPaused = false;
        }

        private void Mute(AudioPoolItem poolItem, bool mute)
        {
            if (poolItem.audioSource.mute == mute) return;

            poolItem.audioSource.mute = mute;

            (mute ? poolItem.onMute : poolItem.onUnmute)?.Invoke();
        }

        private void StopDelayed(AudioPoolItem poolItem)
        {
            poolItem.timer.Restart(GetDuration(poolItem.audioSource), () => Stop(poolItem));
        }

        private AudioPoolItem GetPoolItem()
        {
            if (_inactivePool.Count > 0)
            {
                return _inactivePool.First();
            }

            if (CanExpand())
            {
                return AddNewPoolItem();
            }

            AudioPoolItem poolItem = GetLeastImportant();
            Stop(poolItem);

            return poolItem;
        }

        private bool CanExpand() => _autoExpand && _pool.Count < _maxSize;

        private AudioPoolItem GetLeastImportant()
        {
            AudioPoolItem leastImportant = null;

            int lowestPriority = 0;
            foreach (var keyValuePair in _activePool)
            {
                if (keyValuePair.Value.settings.priority > lowestPriority)
                {
                    lowestPriority = keyValuePair.Value.settings.priority;
                    leastImportant = keyValuePair.Value;
                }
            }

            if (leastImportant == null)
            {
                return _activePool.Last().Value;
            }

            return leastImportant;
        }

        private float GetDurationByPitch(float normalDuration, float pitch)
        {
            float clampedPitch = Mathf.Clamp(Mathf.Abs(pitch), MIN_PITCH, 3f);
            return normalDuration / clampedPitch;
        }

        private float GetDuration(AudioSource source)
        {
            return GetDurationByPitch(source.clip.length, source.pitch);
        }

        private float ClampPitch(float pitch)
        {
            float pitchSign = Mathf.Sign(pitch);
            float absolutePitch = Mathf.Abs(pitch);
            return Mathf.Clamp(absolutePitch, MIN_PITCH, 3f) * pitchSign;
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
