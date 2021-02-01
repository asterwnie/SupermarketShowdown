using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isFinishedInit = false;

    [Header("References")]
    public GameObject groceryItemParent;
    public List<GameObject> possibleGroceryItems;
    public float perlinScale = 15.5f;

    // Objectives & Gamestates ------------------
    [Header("Objectives & Gamestates")]
    public List<GameObject> playerObjectives;
    public List<GameObject> collectedItems;
    public int objectivesToCollect = 4;

    public bool hasWon;
    public bool hasCollectedAllItems;
    public bool isInEndState;
    public bool isChildCaught;
    public bool isMomDoneShopping;
    public AIStates momAIState;

    [Header("Alternative Game Loop Rules")]
    public bool isChildEatInstantly; // this switches on/off whether the child eats instantly, changing the game
    


    // singleton setup
    private void Awake()
    {
        if (instance != null) // if an instance already exists, destroy this (and use the other one instead)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        hasWon = false;
        isChildCaught = false;
        isMomDoneShopping = false;

        // spawn all the store items!!! (this may take a while)
        SpawnGroceries();

        // randomly choose items as objectives
        InitializeObjectives();


        // once finished, notify other things to start
        StartCoroutine(FinishInitAfterDelay(2f));


    }

    IEnumerator FinishInitAfterDelay(float sec)
    {
        yield return new WaitForSeconds(sec);
        isFinishedInit = true;
    }

    void SpawnGroceries()
    {
        GameObject[] spawnTargets = GameObject.FindGameObjectsWithTag("GrocerySpawnTarget");

        for(int i = 0; i < spawnTargets.Length; i++)
        {
            Vector3 targetPos = spawnTargets[i].transform.position;
            float perlinVal = PerlinNoise.GetPerlinValueFromPos(Mathf.RoundToInt(targetPos.x * 100f) , Mathf.RoundToInt(targetPos.y * 100f) , perlinScale); // CAN CURVE IF WE WANT CERTAIN ITEMS ON THE EDGES!!!!???***
            int itemIndexToSpawn = Mathf.FloorToInt(perlinVal * possibleGroceryItems.Count);
            //Debug.Log(perlinVal + " * " + possibleGroceryItems.Count + " = " + (perlinVal * possibleGroceryItems.Count));
            GameObject spawnedGrocery = Instantiate(possibleGroceryItems[itemIndexToSpawn], groceryItemParent.transform);
            spawnedGrocery.transform.position = spawnTargets[i].transform.position;
            spawnedGrocery.transform.rotation = spawnTargets[i].transform.rotation; // face the right direction so the grocery target is in the right place

        }
    }


    void InitializeObjectives()
    {
        // randomly choose items as objectives
        GameObject[] groceries = GameObject.FindGameObjectsWithTag("StoreItem");
        List<int> indiciesUsed = new List<int>();
        if (groceries.Length < objectivesToCollect)
            objectivesToCollect = groceries.Length;
        for (int i = 0; i < objectivesToCollect; i++)
        {
            int rand = Mathf.FloorToInt(Random.value * (groceries.Length - 1));
            while (indiciesUsed.Contains(rand))
            {
                rand = Mathf.FloorToInt(Random.value * (groceries.Length - 1));
            }
            indiciesUsed.Add(rand);

            // mark all the player objectives so they can use the right texture + spawn particles
            playerObjectives.Add(groceries[rand]);
            groceries[rand].GetComponent<PickupItem>().isObjective = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!hasCollectedAllItems)
            CheckIfAllObjectivesCollected();
        if(!isInEndState)
            CheckIfShouldEnterEndState();
        else 
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Movement>().canMove = false;

            // If end state has been reached:
            //Debug.Log("Player won: " + hasWon);
            // execute end-game UI...
        }
    }

    void CheckIfAllObjectivesCollected()
    {
        int collectedCount = 0;
        for(int i = 0; i < playerObjectives.Count; i++)
        {
            if (collectedItems.Contains(playerObjectives[i]))
                collectedCount++;
        }

        if (collectedCount == playerObjectives.Count)
        {
            hasCollectedAllItems = true;
            //Debug.Log("Player has collected all items!");
        }
    }

    void CheckIfShouldEnterEndState()
    {
        if(isChildEatInstantly)
        {
            // THIS IS THE VERSION REFLECTED IN THE VDD
            if(hasCollectedAllItems)
            {
                hasWon = true;
                isInEndState = true;
            }

            if(isChildCaught)
            {
                isInEndState = true;
            }

            if(isInEndState)
            {
                GameObject mom = GameObject.FindGameObjectWithTag("MomNPC");
                mom.GetComponent<PathfindingUnit>().ForceStopPathing();
            }

        }
        else
        {
            // THIS IS THE ALTERNATE VERSION

            // check if the player was caught after the mom finished shopping
            if(isChildCaught && isMomDoneShopping)
            {
                isInEndState = true; 

                // lock player inputs
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Movement>().canMove = false;
                GameObject.Find("PlayerActionManager").GetComponent<PlayerActionManager>().canPerformActions = false;

                GameObject mom = GameObject.FindGameObjectWithTag("MomNPC");
                GameObject cart = GameObject.FindGameObjectWithTag("Cart");
                GameObject cartHandle = GameObject.Find("CartHandle");
                GameObject exitTarget = GameObject.Find("ExitTarget");

                //move mom and player to checkout area
                StartCoroutine(ExecuteCheckoutRoutine(player, mom, cart, cartHandle, exitTarget));
            }
        }

    }

    IEnumerator ExecuteCheckoutRoutine(GameObject player, GameObject mom, GameObject cart, GameObject cartHandle, GameObject exitTarget)
    {
        //Debug.Log("ENTERING END STATE ROUTINE (CHECKING OUT)");

        // check if player won
        if (hasCollectedAllItems)
        {
            hasWon = true;
        }


        // player and mom walk to the cart from MomAIStateMachine
        bool hasReachedCart = false;
        while(!hasReachedCart)
        {
            Debug.Log(Vector3.Distance(mom.transform.position, cart.transform.position));
            if(Vector3.Distance(mom.transform.position, cart.transform.position) < 5.5f && Vector3.Distance(player.transform.position, cart.transform.position) < 5.5f)
            {
                //Debug.Log("Mom and child reached cart.");
                hasReachedCart = true;
            }

            yield return null;
        }

        // after reaching the cart, walk out to the exit (this might look ridiculous lmao)
        //Debug.Log("Pathing mom, player, and cart to the exit...");
        player.GetComponent<PathfindingUnit>().speed = 6f;
        //mom.GetComponent<PathfindingUnit>().speed = 5f;
        cart.GetComponent<PathfindingUnit>().speed = 6f;
        player.GetComponent<PathfindingUnit>().PathTo(exitTarget.transform);
        mom.GetComponent<PathfindingUnit>().ForceStopPathing();
        cart.GetComponent<PathfindingUnit>().PathTo(exitTarget.transform);

        GameObject handle = GameObject.Find("CartTarget");
        while (Vector3.Distance(cart.transform.position, exitTarget.transform.position) > 5f)
        {
            mom.transform.position = Vector3.MoveTowards(mom.transform.position, handle.transform.position, 1f);
            yield return null;
        }

        mom.GetComponent<PathfindingUnit>().ForceStopPathing();
        player.GetComponent<PathfindingUnit>().ForceStopPathing();
        cart.GetComponent<PathfindingUnit>().ForceStopPathing();
       // Debug.Log("End sequence complete.");
        yield return null;
    }

    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
