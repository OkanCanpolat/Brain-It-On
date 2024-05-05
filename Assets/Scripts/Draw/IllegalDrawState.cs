using UnityEngine;
using Zenject;

public class IllegalDrawState : IDrawState
{
    private DrawManager drawManager;
    private Camera mainCam;
    private LineRenderer illegalLine;
    [Inject(Id = "Drawing")] private IDrawState drawingState;
    [Inject(Id = "BeforeDraw")] private IDrawState beforeDrawState;
    [Inject(Id = "Locked")] private IDrawState lockedState;

    public IllegalDrawState(DrawManager drawManager, LineRenderer illegalLine, SignalBus signalBus)
    {
        this.drawManager = drawManager;
        this.illegalLine = illegalLine;
        mainCam = Camera.main;
        signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinished);

    }
    public void OnEnter()
    {

        illegalLine.gameObject.SetActive(true);
        Vector2 lastLinePoint = drawManager.CurrentLine.LineRenderer.GetPosition(drawManager.CurrentLine.PointCount - 1) + drawManager.CurrentLine.transform.position;
        illegalLine.positionCount = 2; 
        illegalLine.SetPosition(0, lastLinePoint);
    }

    public void OnExit()
    {
        illegalLine.gameObject.SetActive(false);
    }

    public void OnUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            illegalLine.SetPosition(1, mousePos);

            Vector2 direction = (illegalLine.GetPosition(1) - illegalLine.GetPosition(0));
            float magnitude = direction.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(illegalLine.GetPosition(0), direction.normalized, magnitude, drawManager.CantDrawLayer);

            if (hit.collider == null)
            {
                drawManager.StateMachine.ChangeState(drawingState);
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
