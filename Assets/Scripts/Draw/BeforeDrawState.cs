using UnityEngine;
using Zenject;

public class BeforeDrawState : IDrawState
{
    private Line linePrefab;
    private DrawManager drawManager;
    private SignalBus signalBus;
    private Camera mainCam;
    private const float raycastRadius = 0.6f;
    [Inject(Id = "Drawing")] private IDrawState drawingState;
    [Inject(Id = "Locked")] private IDrawState lockedState;

    public BeforeDrawState(DrawManager drawManager, SignalBus signalBus, Line linePrefab)
    {
        this.drawManager = drawManager;
        this.signalBus = signalBus;
        this.linePrefab = linePrefab;
        mainCam = Camera.main;
        signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinished);
    }
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hitCircle = Physics2D.CircleCast(mousePos, drawManager.LineWidth * raycastRadius, Vector2.zero, 1f, drawManager.CantDrawLayer);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hitCircle.collider != null) return;
            if (hit.collider == null) return;
            if (hit.transform.gameObject.layer != drawManager.CanDrawLayerIndex) return;

            signalBus.TryFire<OnDrawSignal>();
            drawManager.CurrentLine = Object.Instantiate(linePrefab, new Vector3(mousePos.x, mousePos.y, mainCam.transform.position.z + 1f), Quaternion.identity);
            drawManager.StateMachine.ChangeState(drawingState);
        }
    }

    private void OnLevelFinished()
    {
        drawManager.StateMachine.ChangeState(lockedState);
    }
}
