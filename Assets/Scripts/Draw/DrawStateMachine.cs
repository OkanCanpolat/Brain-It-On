using Zenject;

public class DrawStateMachine
{
    public IDrawState CurrentState;

    public DrawStateMachine([Inject(Id = "BeforeDraw")] IDrawState initialState)
    {
        CurrentState = initialState;
        CurrentState.OnEnter();
    }

    public void ChangeState(IDrawState state)
    {
        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }
}
