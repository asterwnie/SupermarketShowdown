using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MomAIStateMachine : AIStateMachine
{
    public GameObject triggerZone;
    public GameObject cartObj;
    public GameObject cartHandle;
    public List<GameObject> groceryObjectives = new List<GameObject>();
    public List<GameObject> itemsCollected = new List<GameObject>();
    public LayerMask layerMask;
    public float sightRadius = 30f;

    public GameObject thoughtBubbleUI;
    public Text thoughtBubbleText;

    public GameObject playerThoughtBubbleUI;
    public Text playerThoughtBubbleText;

    private void Start()
    {
        StartStateMachine();
        playerThoughtBubbleUI.SetActive(false);
        thoughtBubbleUI.SetActive(false);
    }

    private void Update()
    {
        
        GameManager.instance.momAIState = currentState;

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

        //currentStateObject = new ShoppingState();
        gameObject.AddComponent<ShoppingState>();
        currentStateObject = GetComponent<ShoppingState>();
        currentStateObject.stateMachine = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentStateObject != null)
            currentStateObject.OnTriggerAction(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentStateObject != null)
            currentStateObject.OnTriggerStayAction(other);
    }
}

public class ShoppingState : AIState
{
    public GameObject currentTarget;
    int currentTargetIndex;
    bool isWalking;
    bool reachedItem;
    bool playerNearby;

    private void Start()
    {
        type = AIStates.SHOPPING;
        isWalking = false;
        reachedItem = false;
        playerNearby = false;
        OnEntrance();
    }

    public override void OnEntrance()
    {
        // pick an item on the shopping list we haven't gotten yet
        currentTargetIndex = 0;
        for (int i = currentTargetIndex; i < ((MomAIStateMachine)stateMachine).groceryObjectives.Count; i++)
        {
            currentTargetIndex = i;
            if (((MomAIStateMachine)stateMachine).itemsCollected.Contains(((MomAIStateMachine)stateMachine).groceryObjectives[i]))
            {
                //skip if already collected
                continue;
            }
            else
            {
                currentTarget = ((MomAIStateMachine)stateMachine).groceryObjectives[i];
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
        if(!isWalking)
        {
            isWalking = true;
            GetComponent<PathfindingUnit>().PathTo(currentTarget.transform);
        }

        // if moving onto a next state, return a new state
        if(reachedItem)
        {
            OnExit();
            //return new InspectingState();
            var nextState = gameObject.AddComponent<InspectingState>();
            nextState.stateMachine = this.stateMachine;
            return nextState;
        }

        if(playerNearby)
        {
            OnExit();
            //return new PursueState();
            var nextState = gameObject.AddComponent<PursueState>();
            nextState.stateMachine = this.stateMachine;
            return nextState;
        }

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other)
    {
        // if we've reached our grocery item, change state
        if(((MomAIStateMachine)stateMachine).groceryObjectives.Contains(other.gameObject))
        {
            reachedItem = true;
            ((MomAIStateMachine)stateMachine).itemsCollected.Add(other.gameObject);
        }

        if(other.gameObject.tag == "Player")
        {
            playerNearby = true;
        }
    }

    public override void OnTriggerStayAction(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerNearby = true;
        }
    }

}

public class InspectingState : AIState
{
    public float timeElapsed;
    public float timeToWait;

    private void Start()
    {
        type = AIStates.THINKING;
        timeElapsed = 0;
        //timeToWait = Random.value * 3f + 3f; // produces a random value between 3 and 6
        timeToWait = 5f;
        OnEntrance();
    }

    public override void OnEntrance()
    {
        // play mom's inspecting animation

        // spawn little thinking bubble above head
        ((MomAIStateMachine)stateMachine).thoughtBubbleUI.SetActive(true);
        ((MomAIStateMachine)stateMachine).thoughtBubbleText.text = "..?";

        isInited = true;
    }

    public override void OnExit()
    {
        isInited = false;
        // after inspecting, switch animations
        // .....

        // destroy thinking bubble
        // .....
        ((MomAIStateMachine)stateMachine).thoughtBubbleUI.SetActive(false);

        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    public override AIState UpdateState()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= timeToWait)
        {
            // if there's still more shopping to do, continue shopping
            if(((MomAIStateMachine)stateMachine).groceryObjectives.Count != ((MomAIStateMachine)stateMachine).itemsCollected.Count)
            {
                // switch to shopping after waiting for a bit
                OnExit();
                //return new ShoppingState(); // if moving onto a next state, return a new state
                var nextState = gameObject.AddComponent<ShoppingState>();
                nextState.stateMachine = this.stateMachine;
                return nextState;
            }
            else // otherwise, enter patroling state
            {
                GameManager.instance.isMomDoneShopping = true;
                OnExit();
                //return new PatrolState(); // if moving onto a next state, return a new state
                var nextState = gameObject.AddComponent<PatrolState>();
                nextState.stateMachine = this.stateMachine;
                return nextState;
            }
            
        }

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other)
    {
        
    }

    public override void OnTriggerStayAction(Collider other)
    {

    }

}

public class PursueState : AIState
{
    GameObject player;
    PathfindingUnit pathfindingUnit;
    Vector3 targetLocation;
    public bool caughtPlayer;
    bool isPathingToCart;

    float momHeightOffset = 2f;
    float playerHeightOffset = 1f;

    public float timeRunning;
    public float maxTimeRunning = 7f; // how long the mom will pursue before getting tired

    private void Start()
    {
        type = AIStates.PURSUE;
        caughtPlayer = false;
        player = GameObject.FindGameObjectWithTag("Player");
        pathfindingUnit = GetComponent<PathfindingUnit>();
        timeRunning = 0f;
        isPathingToCart = false;
        OnEntrance();
    }

    public override void OnEntrance()
    {
        // play shocked animation & spawn exclamation point above head
        //...
        ((MomAIStateMachine)stateMachine).thoughtBubbleUI.SetActive(true);
        ((MomAIStateMachine)stateMachine).thoughtBubbleText.text = "!!!";


        // run towards player
        //pathfindingUnit.speed = pathfindingUnit.speed * 2;
        pathfindingUnit.PathTo(player.transform);
        targetLocation = player.transform.position;

        isInited = true;
    }

    public override void OnExit()
    {
        isInited = false;
        ((MomAIStateMachine)stateMachine).thoughtBubbleUI.SetActive(false);
        
        StartCoroutine(DestroyAfterDelay(10f));
        this.enabled = false;
    }

    public override AIState UpdateState()
    {
        timeRunning += Time.deltaTime;

        if(caughtPlayer)
        {
            if (GameManager.instance.isChildEatInstantly)
                GameManager.instance.isChildCaught = true;

            // mom walks the player back to the cart
            if (!isPathingToCart)
            {
                //pathfindingUnit.speed *= 1.5f;
                isPathingToCart = true;
                pathfindingUnit.PathTo(((MomAIStateMachine)stateMachine).cartObj.transform);
                StartCoroutine(PlayerFollowMom());
            }

            if (Vector3.Distance(gameObject.transform.position, ((MomAIStateMachine)stateMachine).cartObj.transform.position) < 3f)
            {
                //pathfindingUnit.speed /= 1.5f;
                StartCoroutine(GivePlayerControlAfterDelay(6f));
                //OnExit();

                // keep shopping if not done; if not, enter lose state
                if (((MomAIStateMachine)stateMachine).itemsCollected.Count != ((MomAIStateMachine)stateMachine).groceryObjectives.Count)
                {
                    

                    OnExit();
                    var nextState = gameObject.AddComponent<InspectingState>();
                    nextState.stateMachine = this.stateMachine;
                    return nextState;
                }
                else
                {
                    // *************** GAME END STATE *****************
                    OnExit();
                    GameManager.instance.isChildCaught = true;
                    return null;
                }
                
            }
        }
        else
        {
            if (timeRunning < maxTimeRunning)
            {
                // if the kid is in sight after reaching the last seen position, continue pursuing the kid

                float radiusBoost = 0f;
                // do a raycast to see if the mom can see the kid
                Ray ray = new Ray(gameObject.transform.position + new Vector3(0, momHeightOffset, 0), (player.transform.position + new Vector3(0, playerHeightOffset, 0)) - (gameObject.transform.position + new Vector3(0, momHeightOffset, 0)));
                RaycastHit hit;
                Physics.Raycast(ray, out hit, ((MomAIStateMachine)stateMachine).sightRadius + radiusBoost, ((MomAIStateMachine)stateMachine).layerMask);
                if (hit.collider != null)
                {
                    if (hit.collider.tag != "Player")
                    {
                        radiusBoost = 0;

                        // keep pursuing last player seen location...(let the current path finish)

                        // upon reaching last seen location, get confused
                        if (Vector3.Distance(gameObject.transform.position, targetLocation) < 2f)
                        {
                            // if player wasn't found at last seen location, go to inspecting state

                            // player is obstructed; mom cannot see player so mom will pause for a bit then go back to shopping
                            pathfindingUnit.ForceStopPathing();
                            OnExit();
                            var nextState = gameObject.AddComponent<InspectingState>();
                            nextState.stateMachine = this.stateMachine;
                            return nextState; // if moving onto a next state, return a new state
                        }
                    }
                    else if (hit.collider.tag == "Player")
                    {
                        radiusBoost = 100f;
                        //Debug.Log("Player spotted!");
                        // if mom can see player, path towards player (**** should the mom pause for a second before doing this?****)
                        
                        pathfindingUnit.PathTo(player.transform);
                        targetLocation = player.transform.position;
                        
                            
                    }
                }
            }
            else
            {
                // when mom has gotten tired, go back to pausing
                OnExit();
                //return new InspectingState();
                // if moving onto a next state, return a new state
                var nextState = gameObject.AddComponent<InspectingState>();
                nextState.stateMachine = this.stateMachine;
                return nextState;
            }
        }

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other)
    {
        if(other.tag == "Player")
        {
            caughtPlayer = true;
            player.GetComponent<Movement>().canMove = false;
        }
    }

    public override void OnTriggerStayAction(Collider other)
    {
        if (other.tag == "Player")
        {
            caughtPlayer = true;
            player.GetComponent<Movement>().canMove = false;
        }
    }

    IEnumerator GivePlayerControlAfterDelay(float seconds)
    {
        float timeElapsed = 0;

        while (timeElapsed < seconds)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > seconds - (seconds - 1.5f))
            {
                Quaternion lookAtCartAngle = Quaternion.LookRotation(new Vector3(((MomAIStateMachine)stateMachine).cartObj.transform.position.x, player.transform.position.y, ((MomAIStateMachine)stateMachine).cartObj.transform.position.z) - player.transform.position, Vector3.up);
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, lookAtCartAngle, Time.deltaTime * 2f);
            }
                //player.transform.LookAt(new Vector3(((MomAIStateMachine)stateMachine).cartObj.transform.position.x, player.transform.position.y, ((MomAIStateMachine)stateMachine).cartObj.transform.position.z));
            yield return null;
        }

        ((MomAIStateMachine)stateMachine).playerThoughtBubbleUI.SetActive(true);
        player.GetComponent<Movement>().canMove = true;

        yield return new WaitForSeconds(3f);
        ((MomAIStateMachine)stateMachine).playerThoughtBubbleUI.SetActive(false);
    }

    IEnumerator PlayerFollowMom()
    {
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<PathfindingUnit>().PathTo(((MomAIStateMachine)stateMachine).cartHandle.transform); // scoot to the side so mom can keep walking
    }

}

public class PatrolState : AIState
{
    GameObject[] patrolPoints;
    PathfindingUnit pathfindingUnit;
    GameObject player;
    public GameObject currentTarget;
    public float distanceToTarget;
    float momHeightOffset = 2f;
    float playerHeightOffset = 1f;

    private void Start()
    {
        type = AIStates.PATROL;
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoints");
        pathfindingUnit = GetComponent<PathfindingUnit>();
        player = GameObject.FindGameObjectWithTag("Player");
        OnEntrance();
    }

    public override void OnEntrance()
    {
        // fetch patrol route points and pick a random one to go to
        currentTarget = patrolPoints[Mathf.RoundToInt(Random.value * (patrolPoints.Length-1))];
        pathfindingUnit.PathTo(currentTarget.transform);

        isInited = true;
    }

    public override void OnExit()
    {
        isInited = false;
        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    public override AIState UpdateState()
    {
        distanceToTarget = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);

        // if the mom can see the kid, immediately stop patroling and pursue player
        Ray ray = new Ray(gameObject.transform.position + new Vector3(0, momHeightOffset, 0), (player.transform.position + new Vector3(0, playerHeightOffset, 0)) - (gameObject.transform.position + new Vector3(0, momHeightOffset, 0)));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, ((MomAIStateMachine)stateMachine).sightRadius, ((MomAIStateMachine)stateMachine).layerMask);
        if (hit.collider != null)
        {
            if(hit.collider.tag == "Player")
            {
                // if mom can see player, path towards player (**** should the mom pause for a second before doing this?****)
                OnExit();
                //return new PursueState(); // if moving onto a next state, return a new state
                var nextState = gameObject.AddComponent<PursueState>();
                nextState.stateMachine = this.stateMachine;
                return nextState;
            }
            else if (Vector3.Distance(gameObject.transform.position, currentTarget.transform.position) < 3f) // if cannot see the kid, keep patroling around
            {
                // fetch patrol route points and pick a random one to go to
                currentTarget = patrolPoints[Mathf.RoundToInt(Random.value * patrolPoints.Length)];
                pathfindingUnit.PathTo(currentTarget.transform);
            }

        }

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other)
    {
       // if(other.gameObject == currentTarget)
        //{
          //  reachedTarget = true;
        //}
    }

    public override void OnTriggerStayAction(Collider other)
    {

    }

}


/* TEMPLATE:
 public class InspectingState : AIState
{

    private void Start() // awake?
    {
        type = AIStates.SHOPPING;
        OnEntrance();
    }

    public override void OnEntrance()
    {
        

        isInited = true;
    }

    public override void OnExit()
    {
        isInited = false;
        StartCoroutine(DestroyAfterDelay());
    }

    public override AIState UpdateState()
    {
       
        // if moving onto a next state, return a new state
        

        // if continuing this state, return itself
        return this;
    }

    public override void OnTriggerAction(Collider other)
    {
        
    }

}
     */

