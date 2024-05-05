using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelFinishUIController : MonoBehaviour
{
    private LevelData levelData;
    private SignalBus signalBus;
    private IScreenRecorderSprite screenRecorder;

    [Header("Panel")]
    [SerializeField] private GameObject levelFinishPanel;
    [SerializeField] private TMP_Text levelIndexText;
    [SerializeField] private TMP_Text goalTimeText;
    [SerializeField] private TMP_Text goalDrawCountText;
    [SerializeField] private TMP_Text solvedTimeText;
    [SerializeField] private TMP_Text solvedDrawCountText;
    [SerializeField] private Image screenShotImage;
    [SerializeField] private GameObject[] stars;

    [Header("Fade")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeTime;
    [SerializeField, Range(0, 1)] private float fadeTargetAlpha;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;

    [Inject]
    public void Construct(SignalBus signalBus, LevelData levelData, IScreenRecorderSprite screenRecorder)
    {
        this.signalBus = signalBus;
        this.levelData = levelData;
        this.screenRecorder = screenRecorder;
    }
    private void Awake()
    {
        goalTimeText.text = string.Format("{0:F1}", levelData.goalTimeSeconds);
        goalDrawCountText.text = levelData.goalDrawCount.ToString();
        levelIndexText.text = levelData.LevelIndex.ToString();
    }

    private void Start()
    {
        signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinished);
        signalBus.Subscribe<LevelFinishedSignal>(OnFinishTimerStop);
        signalBus.Subscribe<LevelFinishDelayTimeChangedSignal>(OnFinishTimerChanged);
        signalBus.Subscribe<FinishTimerStartedSignal>(OnFinishTimerStart);
        signalBus.Subscribe<FinishTimerStoppedSignal>(OnFinishTimerStop);
    }


    private async void OnLevelFinished()
    {
        screenRecorder.RecordScreen();
        await FadeInOut();
        screenShotImage.sprite = screenRecorder.GetResult();

        levelFinishPanel.SetActive(true);
        solvedDrawCountText.text = levelData.SolvedDrawCount.ToString();
        solvedTimeText.text = string.Format("{0:F1}", levelData.SolvedTime);
        CalculateStars();

        if (levelData.SolvedTime < levelData.goalTimeSeconds) solvedTimeText.color = Color.green;
        else solvedTimeText.color = Color.red;

        if (levelData.SolvedDrawCount <= levelData.goalDrawCount) solvedDrawCountText.color = Color.green;
        else solvedDrawCountText.color = Color.red;
    }

    private void CalculateStars()
    {
        int startCount = 0;

        if (levelData.SolvedDrawCount <= levelData.goalDrawCount) startCount++;
        if (levelData.SolvedTime < levelData.goalTimeSeconds) startCount++;

        for(int i = 0; i < startCount; i++)
        {
            stars[i].SetActive(true);
        }
    }
    private async Task FadeInOut()
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;

        float t = 0;

        while(t < fadeTime)
        {
            float remap = (t / fadeTime) * fadeTargetAlpha;
            color.a = remap;
            fadePanel.color = color;
            t += Time.deltaTime;
            await Task.Yield();
        }

        while (t > 0)
        {
            float remap = (t / fadeTime) * fadeTargetAlpha;
            color.a = remap;
            fadePanel.color = color;
            t -= Time.deltaTime;
            await Task.Yield();
        }

        fadePanel.gameObject.SetActive(false);
    }

    private void OnFinishTimerStart()
    {
        timerText.gameObject.SetActive(true);
    }
    private void OnFinishTimerChanged(LevelFinishDelayTimeChangedSignal signal)
    {
        int result = (int)signal.Time;

        timerText.text = result.ToString();
    }
    private void OnFinishTimerStop()
    {
        timerText.gameObject.SetActive(false);
    }
}
