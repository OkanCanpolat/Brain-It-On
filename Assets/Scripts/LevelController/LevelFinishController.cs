using System.Collections;
using UnityEngine;
using Zenject;

public enum LevelFisinsType
{
    Immediate, Delay
}

public class LevelFinishController : MonoBehaviour
{
    [SerializeField] private int levelMissionCount;
    [SerializeField] private LevelFisinsType levelFisinsType;
    private SignalBus signalBus;
    private int remainingMissionCount;
    private const float delayTime = 3f;
    private Coroutine timerCoroutine;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    private void Awake()
    {
        remainingMissionCount = levelMissionCount;
        signalBus.Subscribe<MissionFinishedSignal>(OnMissionFinished);
        signalBus.Subscribe<MissionFinishCancelSignal>(OnCancelMission);
    }

    public void OnMissionFinished()
    {
        remainingMissionCount--;

        if (remainingMissionCount == 0)
        {
            switch (levelFisinsType)
            {
                case LevelFisinsType.Immediate:
                    signalBus.TryFire<LevelFinishedSignal>();
                    break;
                case LevelFisinsType.Delay:
                    signalBus.TryFire<FinishTimerStartedSignal>();
                    timerCoroutine = StartCoroutine(StartDelayTimer());
                    break;
            }
        }
    }
    private void OnCancelMission()
    {
        remainingMissionCount++;
    }
    private IEnumerator StartDelayTimer()
    {
        float t = delayTime;
        LevelFinishDelayTimeChangedSignal signal = new LevelFinishDelayTimeChangedSignal();

        while(t > 0)
        {
            signal.Time = t;
            signalBus.Fire(signal);
            t -= Time.deltaTime;

            if(remainingMissionCount > 0)
            {
                StopCoroutine(timerCoroutine);
                signalBus.TryFire<FinishTimerStoppedSignal>();
            }

            yield return null;
        }


        signalBus.TryUnsubscribe<MissionFinishedSignal>(OnMissionFinished);
        signalBus.TryUnsubscribe<MissionFinishCancelSignal>(OnCancelMission);

        signalBus.TryFire<LevelFinishedSignal>();

    }
}
public class MissionFinishedSignal { }
public class MissionFinishCancelSignal { }
public class LevelFinishedSignal { }
public class LevelFinishDelayTimeChangedSignal
{
    public float Time;
}
public class FinishTimerStartedSignal { }
public class FinishTimerStoppedSignal { }

