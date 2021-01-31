using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericNPCAIStateMachine : AIStateMachine
{
    public int groceriesToCollect;
    public List<GameObject> groceryObjectives = new List<GameObject>();
    public List<GameObject> itemsCollected = new List<GameObject>();
    //public GameObject triggerZone;

    private void Start()
    {
        //populate grocery list
        GameObject[] groceries = GameObject.FindGameObjectsWithTag("StoreItem");
        List<int> indiciesUsed = new List<int>();
        if (groceries.Length < groceriesToCollect)
            groceriesToCollect = groceries.Length;
        for (int i = 0; i < groceriesToCollect; i++)
        {
            int rand = Mathf.FloorToInt(Random.value * (groceries.Length - 1));
            while (indiciesUsed.Contains(rand))
            {
                rand = Mathf.FloorToInt(Random.value * (groceries.Length - 1));
            }
            indiciesUsed.Add(rand);
            if (GameObject.FindGameObjectWithTag("PathfindingGrid").GetComponent<Grid>().NodeFromWorldPoint(groceries[rand].GetComponent<PickupItem>().targetTransform.position).walkable)
                groceryObjectives.Add(groceries[rand]);
        }

        StartStateMachine();
    }

    private void Update()
    {
        if (currentStateObject != null)
        {
            currentState = currentStateObject.GetStateType();

            if (currentStateObject.isInited)
                currentStateObject = currentStateObject.UpdateState(); // will return a new state if the state changes
        }
        else
        {
            currentState = AIStates.IDLE;
        }

    }

    public override void StartStateMachine()
    {
        gameObject.AddComponent<GenericShoppingState>();
        currentStateObject = GetComponent<GenericShoppingState>();
        currentStateObject.stateMachine = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentStateObject != null)
            currentStateObject.OnTriggerAction(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentStateObject != null)
            currentStateObject.OnTriggerStayAction(other);
    }
}


public class GenericShoppingState : AIState
{
    public GameObject currentTarget;
    int currentTargetIndex;
    bool isWalking;
    bool reachedItem;

    private void Start()
    {
        type = AIStates.SHOPPING;
        isWalking = false;
        reachedItem = false;
        OnEntrance();
    }

    public override void OnEntrance()
    {
        // pick an item on the shopping list we haven't gotten yet
        currentTargetIndex = 0;
        for (int i = currentTargetIndex; i < ((GenericNPCAIStateMachine)stateMachine).groceryObjectives.Count; i++)
        {
            currentTargetIndex = i;
            if (((GenericNPCAIStateMachine)stateMachine).itemsCollected.Contains(((GenericNPCAIStateMachine)stateMachine).groceryObjectives[i]))
            {
                //skip if already collected
                continue;
            }
            else
            {
                currentTarget = ((GenericNPCAIStateMachine)stateMachine).groceryObjectives[i];
                break;
            }
        }

        isInited = true;
    }

    public override void OnExit()
    {
        isInited = false;
        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    public override AIState UpdateState()
    {
        // walk towards current target
        if (!isWalking)
        {
            isWalking = true;
            GetComponent<PathfindingUnit>().PathTo(currentTarget.GetComponent<PickupItem>().targetTransform);
        }

        // if moving onto a next state, return a new state
        if (reachedItem)
        {
            OnExit();
            //return new InspectingState();
            var nextState = gameObject.AddComponent<GenericInspectingState>();
            nextState.stateMachine = this.stateMachine;
            return nextState;
        }

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other)
    {
        // if we've reached our grocery item, change state
        if (((GenericNPCAIStateMachine)stateMachine).groceryObjectives.Contains(other.gameObject))
        {
            reachedItem = true;
            ((GenericNPCAIStateMachine)stateMachine).itemsCollected.Add(other.gameObject);
        }
    }

    public override void OnTriggerStayAction(Collider other)
    {

    }

}

public class GenericInspectingState : AIState
{
    public float timeElapsed;
    public float timeToWait;

    private void Start()
    {
        type = AIStates.THINKING;
        timeElapsed = 0;
        timeToWait = 5f;
        OnEntrance();
    }

    public override void OnEntrance()
    {
        isInited = true;
    }

    public override void OnExit()
    {
        isInited = false;
        // after inspecting, switch animations
        // .....

        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    public override AIState UpdateState()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeToWait)
        {
            // if there's still more shopping to do, continue shopping
            if (((GenericNPCAIStateMachine)stateMachine).groceryObjectives.Count == ((GenericNPCAIStateMachine)stateMachine).itemsCollected.Count)
            {
                // dump the list and do it again
                ((GenericNPCAIStateMachine)stateMachine).itemsCollected = new List<GameObject>();
            }
            // switch to shopping after waiting for a bit
            OnExit();
            //return new ShoppingState(); // if moving onto a next state, return a new state
            var nextState = gameObject.AddComponent<GenericShoppingState>();
            nextState.stateMachine = this.stateMachine;
            return nextState;
        }

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other) { }
    public override void OnTriggerStayAction(Collider other) { }

}