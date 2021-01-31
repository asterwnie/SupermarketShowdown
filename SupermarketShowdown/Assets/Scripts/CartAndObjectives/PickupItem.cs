using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName = "default_pickup_item";
    public bool canPickUp = true;

    public void PickedUp()
    {
        // perform a slight delay
        StartCoroutine(DelayNextPickup());

        if (GameManager.instance.isChildEatInstantly)
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
