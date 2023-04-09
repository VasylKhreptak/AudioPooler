using System.Collections.Generic;
using System.Linq;
using Plugins.AudioPooler.Data;
using Plugins.AudioPooler.Enums;
using Plugins.AudioPooler.Linker;
using Plugins.AudioPooler.Tweening;
using UnityEngine;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;

namespace Plugins.AudioPooler.Core
{
    public partial class AudioPooler : MonoBehaviour
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

        private readonly Dictionary<string, FloatTween> _parameterTweens = new Dictionary<string, FloatTween>();

        private int _idGiver;

        private int CollectionCapacity => _allocateMaxMemory ? _maxSize : _initialSize;

        private Transform _audioListenerTransform;

        private const float MIN_PITCH = 0.05f;

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
            StopTweens(poolItem);
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
            PauseTweens(poolItem);
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
            ResumeTweens(poolItem);
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
            poolItem.timer.Restart(GetDurationByPitch(poolItem.audioSource), () => Stop(poolItem));
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
            foreach (var audioPoolItem in _activePool.Values)
            {
                if (audioPoolItem.settings.priority > lowestPriority)
                {
                    lowestPriority = audioPoolItem.settings.priority;
                    leastImportant = audioPoolItem;
                }
            }

            if (leastImportant == null)
            {
                return _activePool.Last().Value;
            }

            return leastImportant;
        }

        private float GetDurationByPitch(AudioSource source)
        {
            float normalDuration = source.clip.length;
            float sourcePitch = source.pitch;

            bool isMixerPitchSet = false;
            float mixerPitch = 0f;
            if (source.outputAudioMixerGroup != null)
            {
                isMixerPitchSet = source.outputAudioMixerGroup.audioMixer.GetFloat("Pitch" + source.outputAudioMixerGroup.name, out mixerPitch);
            }

            return normalDuration / Mathf.Abs(ClampPitch(sourcePitch * (isMixerPitchSet ? mixerPitch : 1)));
        }

        private void UpdateDuration(AudioPoolItem poolItem)
        {
            if (poolItem.settings.loop) return;

            poolItem.timer.UpdateDuration(GetDurationByPitch(poolItem.audioSource));
        }

        private float ClampPitch(float pitch)
        {
            float pitchSign = Mathf.Sign(pitch);
            float absolutePitch = Mathf.Abs(pitch);
            return Mathf.Max(MIN_PITCH, absolutePitch) * pitchSign;
        }

        private void UpdateAudiosDuration()
        {
            foreach (var audioPoolItem in _activePool.Values)
            {
                UpdateDuration(audioPoolItem);
            }
        }

        private void StopTweens(AudioPoolItem poolItem)
        {
            poolItem.StopTweens();
        }

        private void PauseTweens(AudioPoolItem poolItem)
        {
            poolItem.volumeTween.Pause();
            poolItem.pitchTween.Pause();
            poolItem.spatialBlendTween.Pause();
            poolItem.stereoPanTween.Pause();
            poolItem.reverbZoneMixTween.Pause();
            poolItem.spreadTween.Pause();
            poolItem.minDistanceTween.Pause();
            poolItem.maxDistanceTween.Pause();
        }

        private void ResumeTweens(AudioPoolItem poolItem)
        {
            poolItem.volumeTween.Resume();
            poolItem.pitchTween.Resume();
            poolItem.spatialBlendTween.Resume();
            poolItem.stereoPanTween.Resume();
            poolItem.reverbZoneMixTween.Resume();
            poolItem.spreadTween.Resume();
            poolItem.minDistanceTween.Resume();
            poolItem.maxDistanceTween.Resume();
        }

        #endregion
    }
}
