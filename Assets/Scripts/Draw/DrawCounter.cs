using UnityEngine;
using Zenject;

public class DrawCounter : MonoBehaviour
{
    private LevelData levelData;
    private SignalBus signalBus;
    private DrawCountChangedSignal drawCountChangedSignal;

    [Inject]
    public void Construct(SignalBus signalBus, LevelData levelData)
    {
        this.signalBus = signalBus;
        this.levelData = levelData;
    }

    private void Awake()
    {
        signalBus.Subscribe<OnDrawSignal>(OnDraw);
        signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinished);
        drawCountChangedSignal = new DrawCountChangedSignal();
       
    }
    private void Start()
    {
        signalBus.TryFire(drawCountChangedSignal);
    }

    private void OnDraw()
    {
        drawCountChangedSignal.DrawCount++;

        signalBus.TryFire(drawCountChangedSignal);
    }
    private void OnLevelFinished()
    {
        levelData.SolvedDrawCount = drawCountChangedSignal.DrawCount;
        signalBus.Unsubscribe<LevelFinishedSignal>(OnLevelFinished);
        signalBus.Unsubscribe<OnDrawSignal>(OnDraw);
    }
}

public class DrawCountChangedSignal
{
    public int DrawCount;

    public DrawCountChangedSignal()
    {
        DrawCount = 0;
    }
}
