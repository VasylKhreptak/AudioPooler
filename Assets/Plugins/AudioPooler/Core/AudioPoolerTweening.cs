using Plugins.AudioPooler.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.AudioPooler.Core
{
    public partial class AudioPooler
    {
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
                        poolItem.timer.UpdateDuration(GetDurationByPitch(poolItem.audioSource));
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

        public void SetParameterSmooth(AudioMixer mixer, string name, float value, float time, AnimationCurve curve = null)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween floatTween))
            {
                floatTween
                    .Reset()
                    .Getter(() =>
                    {
                        mixer.GetFloat(name, out var x);
                        return x;
                    })
                    .Setter(x => mixer.SetFloat(name, x))
                    .To(value)
                    .Duration(time)
                    .Curve(curve)
                    .Play();
            }
            else
            {
                _parameterTweens.Add(name, new FloatTween());
                SetParameterSmooth(mixer, name, value, time, curve);
            }
        }

        public void SetTrackVolume01Smooth(AudioMixer mixer, string name, float volume01, float time, AnimationCurve curve = null)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween floatTween))
            {
                floatTween
                    .Reset()
                    .Getter(() =>
                    {
                        mixer.GetFloat(name, out var x);
                        return FromDbTo01(x);
                    })
                    .Setter(x => mixer.SetFloat(name, From01ToDb(x)))
                    .To(volume01)
                    .Duration(time)
                    .Curve(curve)
                    .Play();
            }
            else
            {
                _parameterTweens.Add(name, new FloatTween());
                SetTrackVolume01Smooth(mixer, name, volume01, time, curve);
            }
        }

        public void SetTrackVolumeDbSmooth(AudioMixer mixer, string name, float volumeDb, float time, AnimationCurve curve = null)
        {
            SetParameterSmooth(mixer, name, volumeDb, time, curve);
        }

        public void SetTrackPitchSmooth(AudioMixer mixer, string name, float pitch, float time, AnimationCurve curve = null)
        {
            if (_parameterTweens.TryGetValue(name, out FloatTween floatTween))
            {
                pitch = Mathf.Max(0.01f, pitch);

                floatTween
                    .Reset()
                    .Getter(() =>
                    {
                        mixer.GetFloat(name, out var x);
                        return x;
                    })
                    .Setter(x =>
                    {
                        mixer.SetFloat(name, x);
                        UpdateAudiosDuration();
                    })
                    .To(pitch)
                    .Duration(time)
                    .Curve(curve)
                    .Play();
            }
            else
            {
                _parameterTweens.Add(name, new FloatTween());
                SetTrackPitchSmooth(mixer, name, pitch, time, curve);
            }
        }
    }
}
