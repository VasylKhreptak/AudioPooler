using NaughtyAttributes;
using UnityEngine;
using Timer = Plugins.AudioPooler.TimeManagement.Timer;

public class TimerTest : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private float _duration = 3f;

    private Timer _timer = new Timer();

    [Button("Start Timer")]
    public void StartTimer()
    {
        _timer.Start(_duration, () => Debug.Log("Timer finished"));
    }

    [Button("Stop Timer")]
    public void StopTimer()
    {
        _timer.Stop();
    }

    [Button("Restart Timer")]
    public void Restart()
    {
        _timer.Restart(_duration, () => Debug.Log("Timer finished"));
    }


    [Button("Pause Timer")]
    public void Pause() => _timer.Pause();

    [Button("Resume Timer")]
    public void Resume() => _timer.Resume();

    [Button("Toggle Pause")]
    public void TogglePause() => _timer.TogglePause();
}
