using UnityEngine;
using Zenject;

public class DrawingState : IDrawState
{
    private DrawManager drawManager;
    private Camera mainCam;
    private const float raycastRadius = 0.6f;
    [Inject(Id = "Illegal")] private IDrawState illegalDrawState;
    [Inject(Id = "BeforeDraw")] private IDrawState beforeDrawState;
    [Inject(Id = "Locked")] private IDrawState lockedState;

    public DrawingState(DrawManager drawManager, SignalBus signalBus)
    {
        this.drawManager = drawManager;
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
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hitCircle = Physics2D.CircleCast(mousePos, drawManager.LineWidth * raycastRadius, Vector2.zero, 1f, drawManager.CantDrawLayer);
            Vector2 lastPoint = drawManager.CurrentLine.PointCount == 0 ? drawManager.CurrentLine.transform.position : drawManager.CurrentLine.LineRenderer.GetPosition(drawManager.CurrentLine.PointCount - 1) + drawManager.CurrentLine.transform.position;
            Vector2 ray = mousePos - lastPoint;
            RaycastHit2D hit = Physics2D.Raycast(lastPoint, ray.normalized, ray.magnitude, drawManager.CantDrawLayer);

            if ((hit.collider != null) || hitCircle.collider != null)
            {
                drawManager.StateMachine.ChangeState(illegalDrawState);
            }
            else
            {
                drawManager.CurrentLine.AddPosition(mousePos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            drawManager.EndDraw();
            drawManager.StateMachine.ChangeState(beforeDrawState);
        }
    }

    private void OnLevelFinished()
    {
        drawManager.StateMachine.ChangeState(lockedState);
    }
}
