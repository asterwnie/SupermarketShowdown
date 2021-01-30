using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName = "default_pickup_item";
    public bool isObjective = false; // ADD LATER: ADD GLOWY STAR BACKGROUND OR PARTICLES TO INDICATE IT'S AN OBJECTIVE - GODRAYS?
    public bool canPickUp = true;

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

    private void OnCollisionStay(Collision collision)
    {
        if (GetComponent<Rigidbody>().useGravity == false)
            GetComponent<Rigidbody>().useGravity = true;
    }

}
