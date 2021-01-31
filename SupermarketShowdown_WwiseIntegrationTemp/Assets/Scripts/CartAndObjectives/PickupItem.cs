using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Material DEBUGOBJECTIVEMATERIAL;
    public string itemName = "default_pickup_item";
    public Transform targetTransform;
    public bool isObjective = false; // ADD LATER: ADD GLOWY STAR BACKGROUND OR PARTICLES TO INDICATE IT'S AN OBJECTIVE - GODRAYS?
    public bool canPickUp = true;

    bool canInit = false;
    bool gameIsInitted = false;

    private void Update()
    {
        if (!gameIsInitted && GameManager.instance.isFinishedInit)
        {
            gameIsInitted = true;
            canInit = true;
        }

        //only init once
        if(canInit)
        {
            canInit = false;

            if(isObjective)
            {
                // SPAWN PARTICLES ***********


                // SWITCH TO SPECIAL TEXTURE/MATERIAL ***************
                // USE A LOOKUP TABLE
                gameObject.GetComponent<MeshRenderer>().material = DEBUGOBJECTIVEMATERIAL;
            }
        }
    }

    public void PickedUp()
    {
        // perform a slight delay
        StartCoroutine(DelayNextPickup());

        if (GameManager.instance.isChildEatInstantly && isObjective)
        {
            GameManager.instance.collectedItems.Add(this.gameObject);
            this.gameObject.SetActive(false);
            return;
        }
    }

    IEnumerator DelayNextPickup()
    {
        canPickUp = false;
        yield return new WaitForSeconds(0.5f);
        canPickUp = true;
    }
    
}
