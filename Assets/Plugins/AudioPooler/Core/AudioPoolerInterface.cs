using System.Linq;
using Plugins.AudioPooler.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using AudioSettings = Plugins.AudioPooler.Data.AudioSettings;


namespace Plugins.AudioPooler.Core
{
    public partial class AudioPooler
    {
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

        public bool IsActive(int ID)
        {
            return _activePool.ContainsKey(ID);
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
                poolItem.timer.UpdateDuration(GetDurationByPitch(poolItem.audioSource));
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
            foreach (AudioPoolItem poolItem in _activePool.Values)
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
            foreach (AudioPoolItem poolItem in _activePool.Values)
            {
                Pause(poolItem);
            }
        }

        public void ResumeAll()
        {
            foreach (AudioPoolItem poolItem in _activePool.Values)
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

        public float From01ToDb(float volume01)
        {
            volume01 = Mathf.Max(0.0001f, volume01);

            return 20f * Mathf.Log10(volume01);
        }

        public float FromDbTo01(float volumeDb)
        {
            return Mathf.Pow(10f, volumeDb / 20f);
        }

        public void SetTrackVolumeDb(AudioMixer mixer, string name, float volumeDb)
        {
            SetParameter(mixer, name, volumeDb);
        }

        public void SetTrackVolume01(AudioMixer mixer, string name, float volume01)
        {
            SetTrackVolumeDb(mixer, name, From01ToDb(volume01));
        }

        public void SetParameter(AudioMixer mixer, string name, float value)
        {
            mixer.SetFloat(name, value);
        }

        public void StopUpdatingParameter(string name)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween tween))
            {
                tween.Stop();
            }
        }

        public void PauseUpdatingParameter(string name)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween tween))
            {
                tween.Pause();
            }
        }

        public void ResumeUpdatingParameter(string name)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween tween))
            {
                tween.Resume();
            }
        }

        public void TogglePauseUpdatingParameter(string name)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween tween))
            {
                tween.TogglePause();
            }
        }

        public void SetTrackPitch(AudioMixer mixer, string name, float pitch)
        {
            pitch = Mathf.Max(0.01f, pitch);
            mixer.SetFloat(name, pitch);
            UpdateAudiosDuration();
        }
        
        public void StopUpdatingParameters()
        {
            foreach (FloatTween tween in _parameterTweens.Values)
            {
                tween.Stop();
            }
        }
        
        public void PauseUpdatingParameters()
        {
            foreach (FloatTween tween in _parameterTweens.Values)
            {
                tween.Pause();
            }
        }
        
        public void ResumeUpdatingParameters()
        {
            foreach (FloatTween tween in _parameterTweens.Values)
            {
                tween.Resume();
            }
        }
    }
}
