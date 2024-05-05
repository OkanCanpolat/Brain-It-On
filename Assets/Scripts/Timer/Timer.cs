using System.Collections;
using UnityEngine;
using Zenject;

public class Timer : MonoBehaviour
{
    private LevelData levelData;
    private TimeChangedSignal timeChangedSignal;
    private SignalBus signalBus;
    private Coroutine timerCoroutine;
    private WaitForSeconds delay;
    private const float delayTime = 0.1f;
    private float elapsedTime;

    public float ElapsedTime => elapsedTime;

    [Inject]
    public void Construct(SignalBus signalBus, LevelData levelData)
    {
        this.signalBus = signalBus;
        this.levelData = levelData;
    }
    private void Awake()
    {
        timeChangedSignal = new TimeChangedSignal(elapsedTime);
        delay = new WaitForSeconds(delayTime);

        signalBus.Subscribe<OnDrawSignal>(StartTimer);
        signalBus.Subscribe<FinishTimerStoppedSignal>(StartTimer);
        signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinished);
        signalBus.Subscribe<FinishTimerStartedSignal>(StopTimer);
    }
    private void Start()
    {
        signalBus.TryFire(timeChangedSignal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            signalBus.TryFire<MissionFinishCancelSignal>();
        }
    }
    private IEnumerator StartTimerCo()
    {
        while (true)
        {
            yield return delay;

            elapsedTime += delayTime;
            timeChangedSignal.Time = elapsedTime;
            signalBus.TryFire(timeChangedSignal);
        }
    }
    private void OnLevelFinished()
    {
        StopTimer();
        levelData.SolvedTime = elapsedTime;
        signalBus.Unsubscribe<LevelFinishedSignal>(OnLevelFinished);
    }
    public void StopTimer()
    {
        StopCoroutine(timerCoroutine);
    }
    public void StartTimer()
    {
        timerCoroutine = StartCoroutine(StartTimerCo());
        signalBus.TryUnsubscribe<OnDrawSignal>(StartTimer);
    }
}

public class TimeChangedSignal
{
    public float Time;

    public TimeChangedSignal(float time)
    {
        Time = time;
    }
}
