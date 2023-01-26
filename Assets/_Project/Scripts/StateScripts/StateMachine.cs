using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State CurrentState { get; private set; }

    void Start()
    {
        CurrentState = GetInitialState();
        if (CurrentState != null)
        {
            CurrentState.Enter();
        }
    }

    void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateLogic();
        }
    }

    public virtual void ChangeState(State newState)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }
        CurrentState = newState;
        OnStateChange();
        CurrentState.Enter();
    }

    protected virtual void OnStateChange() { }

    protected virtual State GetInitialState()
    {
        return null;
    }
}