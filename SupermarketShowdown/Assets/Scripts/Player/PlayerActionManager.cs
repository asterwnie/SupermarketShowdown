using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Manages player inputs for:
 * - picking up items
 * - attacking items (and playing correct animation/applying dmg depending on item)
 * - executing animations at the right times
 */

public enum PlayerActions
{
    PICKUP,
    COUNT
}

public class PlayerActionManager : MonoBehaviour
{
    // USE THIS TO QUICKLY CHANGE INPUT SCHEME (MAKE SURE TO BIND THEM IN START & HAVING A MATCHING PLAYER ACTION ENUM)
    public static KeyCode[] PlayerInputs = {
        KeyCode.Mouse0, // pickup
        //KeyCode.Mouse0 // drop item
        };

    // lookup table of keycode to find which action it should do (gets bound at runtime)
    public static Dictionary<KeyCode, PlayerActions> PlayerControls = new Dictionary<KeyCode, PlayerActions>();

    [Header("References")]
    public GameObject player;
    public GameObject playerHand;
    public float playerHeight = 3f;
    public float throwForce = 5f;
    public float throwingDelay = 1f;
    bool isThrowing = false;
    Vector3 throwVector;
    public bool canPerformActions = true;
    public GameObject currentHeldItem = null;

    public GameObject spherePathPrefab;
    public GameObject throwPathParent;
    GameObject[] throwDebugObjects;
    Vector3 result; // DEBUG
    float mouseDragDistance; // DEBUG;
    Vector3 mouseStartPos; //DEBUG
    Vector2 displacement; //debug
    float angle;//debug


    [Header("Item Pickups")]
    public List<GameObject> nearbyItems; // pickup items will add themselves to this list in their OnTriggerEnter and remove when exiting
    bool isHoldingItem = false;

    // Start is called before the first frame update
    void Start()
    {
        // bind controls to actions
        PlayerControls.Add(PlayerInputs[(int)PlayerActions.PICKUP], PlayerActions.PICKUP);
        //PlayerControls.Add(PlayerInputs[(int)PlayerActions.DROPITEM], PlayerActions.DROPITEM);
       // PlayerControls.Add(PlayerInputs[(int)PlayerActions.ATTACK], PlayerActions.ATTACK);


        nearbyItems = new List<GameObject>();

        throwDebugObjects = new GameObject[10];
    }

    // Update is called once per frame
    void Update()
    {
        if (canPerformActions)
        {
            foreach (KeyCode input in PlayerInputs)
            {
                if (Input.GetKeyDown(input))
                    PerformPlayerAction(PlayerControls[input]);
            }
        }

        // held item should follow the player's hand
        if (isHoldingItem)
        {
            currentHeldItem.transform.position = playerHand.transform.position;
            currentHeldItem.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        /*if(isThrowing && throwVector != null)
        {
            throwDebugObjects
        }*/

        if(isThrowing)
        {
            displacement = new Vector2(Input.mousePosition.x - mouseStartPos.x, Input.mousePosition.y - mouseStartPos.y);
            angle = Vector2.SignedAngle(Vector2.up, displacement);
            mouseDragDistance = Mathf.Abs(Vector2.Distance(mouseStartPos, Input.mousePosition)) / 100f;
            if (mouseDragDistance >= 10f)
                mouseDragDistance = 10f; // cap distance

            angle = 360-angle + 180 + 45;

            currentHeldItem.transform.rotation = Quaternion.Euler(0, angle, 0);
            throwVector = new Vector3(currentHeldItem.transform.forward.x * throwForce, mouseDragDistance, currentHeldItem.transform.forward.z * throwForce);

            DrawThrowArc();
        }
        else
        {
            throwPathParent.SetActive(false);
        }
        
    }

    private void FixedUpdate()
    {

    }

    public void PerformPlayerAction(PlayerActions action)
    {
        switch (action)
        {
            case PlayerActions.PICKUP:
                if(!isHoldingItem && nearbyItems.Count > 0) // if there are pickupable items nearby
                {
                    AkSoundEngine.PostEvent("Place_Item", gameObject);
                    HoldItem(nearbyItems[0]);
                }
                else if (isHoldingItem) // drop the item
                {
                    float timeElapsed = 0;
                    StartCoroutine(DropOrThrowItem(KeyCode.Mouse0, timeElapsed));
                    /*
                    // re-enable that item's trigger zone
                    currentHeldItem.GetComponent<Collider>().enabled = true;
                    currentHeldItem.transform.position = player.transform.position + player.transform.forward * 2.5f + new Vector3(0, playerHeight, 0); // place in front of player
                    currentHeldItem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 5f, 0)); // bounce it up a little

                    // un-store this item as player holded item
                    isHoldingItem = false;
                    currentHeldItem = null;
    
                    // play default animation
                    // ..........
                    */
                }
                break;

        }
    }

    // PICKUP ---------------------------------------

    public void HoldItem(GameObject item)
    {
        // drop any item player was holding and re-enable that item's trigger zone
        if (isHoldingItem)
        {
            currentHeldItem.GetComponent<Collider>().enabled = true;
            currentHeldItem.transform.position = player.transform.position + player.transform.forward * 2.5f + new Vector3(0, playerHeight, 0); // place in front of player
            currentHeldItem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 5f, 0)); // bounce it up a little
        }

        // store this item as player holded item
        currentHeldItem = item;
        nearbyItems.Remove(currentHeldItem);
        currentHeldItem.GetComponent<PickupItem>().PickedUp(); // notify the item that it has been picked up

        // disable the item's trigger zone
        currentHeldItem.GetComponent<Collider>().enabled = false;

        // make held item follow player's hand
        isHoldingItem = true;

        // play holding animation
        // ...............
    }

    IEnumerator DropOrThrowItem(KeyCode key, float timeElapsed)
    {
        if (isHoldingItem) // drop the item
        {
            bool hasPerformedAction = false;
            mouseStartPos = Input.mousePosition;
            //Vector2 mouseReleasePos;
            while (!hasPerformedAction)
            {
                if (Input.GetKey(key))
                {
                    timeElapsed += Time.deltaTime;
                }

                if (timeElapsed >= throwingDelay) // if held for one second, can start dragging to throw item
                {
                    isThrowing = true;
                    // spawn throwing UI

                    // do throw on release
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        // calculations for throw force are done in update
                        AkSoundEngine.PostEvent("Throw_Item", gameObject);
                        currentHeldItem.GetComponent<Collider>().enabled = true;
                        currentHeldItem.transform.position = player.transform.position + new Vector3(0, playerHeight, 0); // place in front of player
                        isHoldingItem = false;

                        // CHANGE MOUSE DRAG DIST TO NORMALIZE IT BASED ON SCREEN SIZE LATER

                        currentHeldItem.GetComponent<Rigidbody>().velocity = throwVector;
                        currentHeldItem = null;
                        hasPerformedAction = true;

                        isThrowing = false;
                        //hide throwing ui
                    }
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0)) // do a simple drop on release
                {
                    AkSoundEngine.PostEvent("Player_Swipe", gameObject);
                    // re-enable that item's trigger zone
                    currentHeldItem.GetComponent<Collider>().enabled = true;
                    currentHeldItem.transform.position = player.transform.position + player.transform.forward * 2.5f + new Vector3(0, playerHeight, 0); // place in front of player
                    currentHeldItem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 5f, 0)); // bounce it up a little

                    // un-store this item as player holded item
                    isHoldingItem = false;
                    currentHeldItem = null;

                    // play default animation
                    // ..........
                    hasPerformedAction = true;
                }

                yield return null;
            }
        }
        yield return null;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PickupItem" || other.tag == "StoreItem")
        {
            nearbyItems.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "PickupItem" || other.tag == "StoreItem")
        {
            nearbyItems.Remove(other.gameObject);
        }
    }

    
    float timeStep = 0.2f;
    float timeElapsed = 0;
    
    void DrawThrowArc()
    {
        if (throwVector != null && isThrowing)
        {
            throwPathParent.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                Vector3 playerHeadPos = new Vector3(transform.position.x, transform.position.y + playerHeight, transform.position.z);
                timeElapsed += timeStep;
                Vector3 velocityPos = playerHeadPos + new Vector3((throwVector.x * timeElapsed), (timeElapsed * throwVector.y) + (-5f * Mathf.Pow(timeElapsed, 2f)), (throwVector.z * timeElapsed));

                if(throwDebugObjects[i] == null)
                    throwDebugObjects[i] = Instantiate(spherePathPrefab, throwPathParent.transform);
                throwDebugObjects[i].transform.position = velocityPos;
            }
            timeElapsed = 0f;
            return;
        }
        
    }


}

