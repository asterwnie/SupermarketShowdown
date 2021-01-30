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
    DROPITEM,
    COUNT
}

public class PlayerActionManager : MonoBehaviour
{
    // USE THIS TO QUICKLY CHANGE INPUT SCHEME (MAKE SURE TO BIND THEM IN START & HAVING A MATCHING PLAYER ACTION ENUM)
    public static KeyCode[] PlayerInputs = {
        KeyCode.Mouse0, // pickup
        KeyCode.Mouse1 // drop item
        };

    // lookup table of keycode to find which action it should do (gets bound at runtime)
    public static Dictionary<KeyCode, PlayerActions> PlayerControls = new Dictionary<KeyCode, PlayerActions>();

    [Header("References")]
    public GameObject player;
    public GameObject playerHand;
    public float playerHeight = 3f;
    public bool canPerformActions = true;
    public GameObject currentHeldItem = null;
    

    [Header("Item Pickups")]
    public List<GameObject> nearbyItems; // pickup items will add themselves to this list in their OnTriggerEnter and remove when exiting
    bool isHoldingItem = false;
    public GameObject currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        // bind controls to actions
        PlayerControls.Add(PlayerInputs[(int)PlayerActions.PICKUP], PlayerActions.PICKUP);
        PlayerControls.Add(PlayerInputs[(int)PlayerActions.DROPITEM], PlayerActions.DROPITEM);
       // PlayerControls.Add(PlayerInputs[(int)PlayerActions.ATTACK], PlayerActions.ATTACK);


        nearbyItems = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canPerformActions)
        {
            foreach (KeyCode input in PlayerInputs)
            {
                if(Input.GetKeyDown(input))
                    PerformPlayerAction(PlayerControls[input]);
            }
        }

        // held item should follow the player's hand
        if (isHoldingItem)
        {
            currentHeldItem.transform.position = playerHand.transform.position;
            currentHeldItem.transform.rotation = Quaternion.Euler(Vector3.zero);
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
                if(nearbyItems.Count > 0) // if there are pickupable items nearby
                {
                    HoldItem(nearbyItems[0]);
                }
                else if (isHoldingItem) // drop the item
                {
                    // re-enable that item's trigger zone
                    currentHeldItem.GetComponent<Collider>().enabled = true;
                    currentHeldItem.transform.position = player.transform.position + player.transform.forward * 2.5f + new Vector3(0, playerHeight, 0); // place in front of player
                    currentHeldItem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 5f, 0)); // bounce it up a little

                    // un-store this item as player holded item
                    isHoldingItem = false;
                    currentHeldItem = null;
    
                    // play default animation
                    // ..........
                }
                break;
                
            case PlayerActions.DROPITEM:
                float timeElapsed = 0;
                StartCoroutine(DropOrThrowItem(KeyCode.Mouse1, timeElapsed));
                break;

                /*
            case PlayerActions.ATTACK:
                // make sure player isn't holding anything
                // check what kind of weapon it is
                // execute action/animation depending on the weapon
                break;
            case PlayerActions.EQUIP:
                // play equip animation
                // set correct idle animation
                break;*/
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
            Vector2 mouseStartPos = Input.mousePosition;
            Vector2 mouseReleasePos;
            while (!hasPerformedAction)
            {
                if (Input.GetKey(key))
                {
                    timeElapsed += Time.deltaTime;
                }

                if (timeElapsed >= 1f) // if held for one second, can start dragging to throw item
                {
                    // spawn throwing UI

                    // do throw on release
                    if (Input.GetKeyUp(KeyCode.Mouse1))
                    {
                        // calculate throwing angle
                        mouseReleasePos = Input.mousePosition;
                        Vector2 displacement = new Vector2(mouseReleasePos.x - mouseStartPos.x, mouseReleasePos.y - mouseStartPos.y);
                        float angle = Vector2.SignedAngle(Vector2.down, displacement);
                        Debug.Log("Angle to throw: " + angle);
                        hasPerformedAction = true;


                        //hide throwing ui
                    }
                }
                else if (Input.GetKeyUp(KeyCode.Mouse1)) // do a simple drop on release
                {
                    // re-enable that item's trigger zone
                    currentHeldItem.GetComponent<Collider>().enabled = true;
                    currentHeldItem.transform.position = player.transform.position + player.transform.forward * 2.5f + new Vector3(0, playerHeight, 0); // place in front of player

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
        if(other.tag == "PickupItem")
        {
            nearbyItems.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "PickupItem")
        {
            nearbyItems.Remove(other.gameObject);
        }
    }


    
}

