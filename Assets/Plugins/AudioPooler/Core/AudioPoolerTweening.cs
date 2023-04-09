using UnityEngine;

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
    }
}
