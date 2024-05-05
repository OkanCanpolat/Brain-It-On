using TMPro;
using UnityEngine;
using Zenject;

public class IndicatorUIController : MonoBehaviour
{
    private LevelData levelData;
    [SerializeField] private TMP_Text levelIndexText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text levelTargetDrawCountText;
    [SerializeField] private TMP_Text drawCountText;
    [SerializeField] private TMP_Text missionExplanationText;

    private SignalBus signalBus;

    private float levelTime;

    [Inject]
    public void Construct(SignalBus signalBus, LevelData levelData)
    {
        this.signalBus = signalBus;
        this.levelData = levelData;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale = 0;
        }
    }
    private void Awake()
    {
        signalBus.Subscribe<TimeChangedSignal>(OnTimeChanged);
        signalBus.Subscribe<DrawCountChangedSignal>(OnDrawCountChanged);
        signalBus.Subscribe<OnDrawSignal>(OnFirstDraw);

        levelTime = levelData.goalTimeSeconds;
        levelTargetDrawCountText.text = levelData.goalDrawCount.ToString();
        levelIndexText.text = levelData.LevelIndex.ToString();
    }

    public void OnTimeChanged(TimeChangedSignal signal)
    {
        float remainigTime = levelTime - signal.Time;

        if(remainigTime < 0)
        {
            remainigTime = 0;
            signalBus.Unsubscribe<TimeChangedSignal>(OnTimeChanged);
            timeText.color = Color.red;
        }

        timeText.text = string.Format("{0:F1}", remainigTime);
    }
    public void OnDrawCountChanged(DrawCountChangedSignal signal)
    {
        drawCountText.text = signal.DrawCount.ToString();

        if(signal.DrawCount > levelData.goalDrawCount)
        {
            drawCountText.color = Color.red;
        }
    }
    public void OnFirstDraw()
    {
        signalBus.Unsubscribe<OnDrawSignal>(OnFirstDraw);
        missionExplanationText.gameObject.SetActive(false);
    }
}
