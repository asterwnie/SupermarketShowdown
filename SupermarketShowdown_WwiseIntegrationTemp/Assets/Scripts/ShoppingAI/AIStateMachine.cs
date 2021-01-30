using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is the AI state machine template! all state machines/states should inherit from this.

public enum AIStates
{
    WALKING_TO,
    SHOPPING,
    THINKING,
    PURSUE,
    PATROL,
    IDLE
}

public abstract class AIState : MonoBehaviour
{
    public abstract void OnEntrance();
    public abstract void OnExit();
    public abstract AIState UpdateState(); // returns a new state if the state has changed
    public abstract void OnTriggerAction(Collider other);
    public abstract void OnTriggerStayAction(Collider other);

    public AIStates GetStateType() { return type; }
    protected AIStates type;

    public bool isInited;
    public AIStateMachine stateMachine;

    public IEnumerator DestroyAfterDelay(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(this);
    }
}

public abstract class AIStateMachine : MonoBehaviour
{
    //updates will be in the Update of the state machine
    //public abstract bool UpdateStateMachine(); // return true if still running
    public abstract void StartStateMachine();

    public AIStates GetCurrentState() { return currentState; }
    protected AIStates currentState;
    protected AIState currentStateObject;

}
